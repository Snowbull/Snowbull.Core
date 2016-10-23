using System;
using Akka.Actor;
using System.Data.Entity;
using System.Collections.Immutable;

namespace Snowbull.Core.Game {
    public class GameUserActor : UserActor, IWithUnboundedStash {
        private Player.Player player;
        private Rooms.Room room = null;
        private int joining = -1;
        private static readonly ImmutableArray<int> starts = (new int[] {100, 200, 300, 400, 800, 801, 802, 230, 810, 804}).ToImmutableArray(); // Starting rooms.

        public IStash Stash {
            get;
            set;
        }

		public static Props Props(GameUser user) {
			return Akka.Actor.Props.Create(() => new GameUserActor(user));
		}

		public GameUserActor(GameUser user) : base(user) {

		}

        protected override void PreStart() {
            base.PreStart();
            GameUser user = (GameUser) this.user;
            Data.SnowbullContext db = new Data.SnowbullContext();
            db.Users.FirstAsync<Data.Models.User>(u => u.Id == user.Id).ContinueWith<Player.Player>(
                t => {
                    return t.IsFaulted ? null : new Player.Player(
                        user,
                        new Player.Clothing(t.Result.Clothing),
                        new Player.Position(0, 0, 0)
                    );
                }
            ).PipeTo(Self);
        }

		protected override void Running() {
            Receive<Packets.IReceivePacket>(new Action<Packets.IReceivePacket>(StashIncoming));
            Receive<Player.Player>(new Action<Player.Player>(Loaded));
		}

        private void Loaded(Player.Player player) {
            if(player != null) {
                this.player = player;
                BecomeStacked(Ready);
                Stash.UnstashAll();
            }else{
                connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.NO_DB_CONNECTION, -1), Self);
            }
        }

        private void Ready() {
            base.Running();
            Receive<Packets.Xt.Receive.Authentication.JoinServer>(new Action<Packets.Xt.Receive.Authentication.JoinServer>(JoinServer));
            Receive<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(new Action<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(GetBuddies));
            Receive<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(new Action<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(GetIgnored));
            Receive<Packets.Xt.Receive.Player.Inventory.GetInventory>(new Action<Packets.Xt.Receive.Player.Inventory.GetInventory>(GetInventory));
            Receive<Packets.Xt.Receive.GetLastRevision>(new Action<Packets.Xt.Receive.GetLastRevision>(GetLastRevision));
            Receive<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(new Action<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(GetEPFPoints));
            Receive<Packets.Xt.Receive.Heartbeat>(new Action<Packets.Xt.Receive.Heartbeat>(Heartbeat));
        }

        private void Joined() {
            Ready();
            Receive<Packets.Xt.Receive.Rooms.JoinRoom>(new Action<Packets.Xt.Receive.Rooms.JoinRoom>(JoinRoom));
        }

        private void Transitioning() {
            base.Running();
            Receive<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
            Receive<Packets.IReceivePacket>(new Action<Packets.IReceivePacket>(StashIncoming));
        }

        private void StashIncoming(Packets.IReceivePacket received) {
            Stash.Stash();
        }

		private void JoinServer(Packets.Xt.Receive.Authentication.JoinServer js) {
			// We'll check the login key again I GUESS
			// But for now let's just accept it.
			connection.ActorRef.Tell(
				new Packets.Xt.Send.Authentication.JoinServer(
					agent: true, 
					guide: true,
					moderator: false, 
					modifiedStampCover: true
				)
			, Self);
		}

        private void GetBuddies(Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies gb) {
            connection.ActorRef.Tell(new Packets.Xt.Send.Player.Relations.Buddies.GetBuddies(), Self); // TODO - Load actual buddy list.
        }

        private void GetIgnored(Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored gn) {
            connection.ActorRef.Tell(new Packets.Xt.Send.Player.Relations.Ignore.GetIgnored(), Self); // TODO - Load actual ignore list.
        }

        private void GetInventory(Packets.Xt.Receive.Player.Inventory.GetInventory gi) {
            connection.ActorRef.Tell(new Packets.Xt.Send.Player.Inventory.GetInventory(), Self); // TODO - Load actual inventory list.
        }

        private void GetLastRevision(Packets.Xt.Receive.GetLastRevision glr) {
            connection.ActorRef.Tell(new Packets.Xt.Send.GetLastRevision(3239), Self);
            BecomeStacked(Transitioning);
            joining = starts[(new Random()).Next(0, starts.Length)];
            user.Zone.ActorRef.Tell(new Rooms.JoinRoom(joining, player), Self);
        }

        private void GetEPFPoints(Packets.Xt.Receive.Player.EPF.GetEPFPoints epfgr) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.EPF.GetEPFPoints(0, 1) // TODO - Find out what these are?
            , Self);
        }

        private void Heartbeat(Packets.Xt.Receive.Heartbeat h) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Heartbeat(h.Room)
            , Self);
        }

        private void JoinedRoom(JoinedRoom jr) {
            if(joining != -1 && joining != jr.Room.ExternalID) {
                Stash.Stash(); // Stash any joined room messages from other rooms, we need to cleanly finish this transaction.
            }else{
                joining = -1;
                if(room != null) {
                    room.ActorRef.Tell(new Rooms.LeaveRoom((GameUser)user), Self);
                    UnbecomeStacked();
                }else{
                    Become(Joined);
                }
                room = jr.Room;
                connection.ActorRef.Tell(new Packets.Xt.Send.Rooms.JoinedRoom(jr.Room.ExternalID, jr.Players, jr.Room.InternalID), Self);
                Stash.UnstashAll();
            }
        }

        private void RoomFull(RoomFull rf) {
            if(room == null) {
                connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.ROOM_FULL, rf.Room.InternalID));
                joining = -1;
            }else{
                int i = starts.IndexOf(joining);
                joining = starts[(i + 1) % starts.Length];
                user.Zone.ActorRef.Tell(new Rooms.JoinRoom(joining, player), Self);
            }
        }

        private void JoinRoom(Packets.Xt.Receive.Rooms.JoinRoom jr) {
            BecomeStacked(Transitioning);
            joining = jr.ExternalID;
            user.Zone.ActorRef.Tell(new Rooms.JoinRoom(joining, player));
        }
	}

    internal class JoinedRoom {
        public Rooms.Room Room {
            get;
            private set;
        }

        public ImmutableList<Player.Player> Players {
            get;
            private set;
        }

        public JoinedRoom(Rooms.Room room, ImmutableList<Player.Player> players) {
            Room = room;
            Players = players;
        }
    }

    internal class RoomFull {
        public Rooms.Room Room {
            get;
            private set;
        }

        public RoomFull(Rooms.Room room) {
            Room = room;
        }
    }
}

