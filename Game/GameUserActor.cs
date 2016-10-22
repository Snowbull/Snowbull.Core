﻿using System;
using Akka.Actor;
using System.Collections.Immutable;

namespace Snowbull.Core.Game {
    public class GameUserActor : UserActor, IWithUnboundedStash {
        private readonly Player.Player player;
        private Rooms.Room room = null;
        private int joining = -1;

        public IStash Stash {
            get;
            set;
        }

		public static Props Props(GameUser user) {
			return Akka.Actor.Props.Create(() => new GameUserActor(user));
		}

		public GameUserActor(GameUser user) : base(user) {
            player = new Player.Player(user, new Player.Clothing(1, 0, 0, 0, 0, 0, 0, 0, 0), new Position(0, 0, 0)); // Temporary.
		}

		protected override void Running() {
			base.Running();
			Receive<Packets.Xt.Receive.Authentication.JoinServer>(new Action<Packets.Xt.Receive.Authentication.JoinServer>(JoinServer));
            Receive<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(new Action<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(GetBuddies));
            Receive<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(new Action<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(GetIgnored));
            Receive<Packets.Xt.Receive.Player.Inventory.GetInventory>(new Action<Packets.Xt.Receive.Player.Inventory.GetInventory>(GetInventory));
            Receive<Packets.Xt.Receive.GetLastRevision>(new Action<Packets.Xt.Receive.GetLastRevision>(GetLastRevision));
            Receive<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(new Action<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(GetEPFPoints));
		}

        private void Joined() {
            Running();
            Receive<Packets.Xt.Receive.Rooms.JoinRoom>(JoinRoom);
        }

        private void Transitioning() {
            base.Running();
            Receive<JoinedRoom>(JoinedRoom);
            Receive<Packets.IReceivePacket>(StashIncoming);
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
        }

        private void GetEPFPoints(Packets.Xt.Receive.Player.EPF.GetEPFPoints epfgr) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.EPF.GetEPFPoints(0, 1) // TODO - Find out what these are?
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
            connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.ROOM_FULL, rf.Room.InternalID));
            joining = -1;
        }

        private void JoinRoom(Packets.Xt.Receive.Rooms.JoinRoom jr) {
            BecomeStacked(Transitioning);
            joining = jr.ExternalID;
            user.Zone.ActorRef.Tell(new Rooms.JoinRoom(jr, player));
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

