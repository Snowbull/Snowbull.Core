/**
 * Game user actor for Snowbull.
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of Snowbull.
 * 
 * Snowbull is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * Snowbull is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Snowbull. If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;
using Akka.Actor;
using Akka.Persistence;
using System.Data.Entity;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Snowbull.Core.Game {
    public class GameUserState {
        public Player.Player Player {
            get;
        }

        public ImmutableDictionary<int, Player.Clothing.Item> Inventory {
            get;
        }

        public Rooms.Room Room {
            get;
        }

        public GameUserState(Player.Player player, ImmutableDictionary<int, Player.Clothing.Item> inventory, Rooms.Room room) {
            Player = player;
            Room = room;
            Inventory = inventory;
        }

        public GameUserState UpdatePlayer(Player.Player player) {
            return new GameUserState(player, Inventory, Room);
        }

        public GameUserState UpdateInventory(ImmutableDictionary<int, Player.Clothing.Item> inventory) {
            return new GameUserState(Player, inventory, Room);
        }

        public GameUserState UpdateRoom(Rooms.Room room) {
            return new GameUserState(Player, Inventory, room);
        }
    }

    public class GameUserActor : UserActor, IWithUnboundedStash {
        private GameUserState state;
        private static readonly ImmutableArray<int> starts = (new int[] {100, 200, 300, 400, 800, 801, 802, 230, 810, 804}).ToImmutableArray(); // Starting rooms.
        private ImmutableDictionary<int, Player.Clothing.Item> items;

        private GameUserState State {
            get { return state; }
            set {
                SaveSnapshot(value);
                state = value;
            }
        }

        private Player.Player Player {
            get { return state.Player; }
        }

        private ImmutableDictionary<int, Player.Clothing.Item> Inventory {
            get { return state.Inventory; }
        }

        private Rooms.Room Room {
            get { return state.Room; }
        }

        /// <summary>
        /// Sets up props for the new actor.
        /// </summary>
        /// <param name="user">Immutable user context.</param>
        public static Props Props(GameUser user, ImmutableDictionary<int, Player.Clothing.Item> items) {
			return Akka.Actor.Props.Create(() => new GameUserActor(user, items));
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.GameUserActor"/> class.
        /// </summary>
        /// <param name="user">Immutable user context.</param>
        public GameUserActor(GameUser user, ImmutableDictionary<int, Player.Clothing.Item> items) : base(user, string.Format("user(Id={0},Username={1})", user.Id, user.Username)) {
            this.items = items;
		}

        /// <summary>
        /// Pre-start routine.
        /// </summary>
        protected override void PreStart() {
            base.PreStart(); // I suppose.
            GameUser user = (GameUser) this.user; // Keep a local copy, just in case... something.
            Data.SnowbullContext db = new Data.SnowbullContext();
            // Load the user from the database.
            db.Users.Include("Clothing").Include("Inventory").FirstAsync<Data.Models.User>(u => u.Id == user.Id).ContinueWith<Loaded>(
                t => {
                    db.Dispose();
                    if(t.IsFaulted) {
                        return null;
                    }else{
                        Player.Player player = new Player.Player(
                            user,
                            new Player.Clothing.Costume(t.Result.Clothing, items),
                            new Player.Position(0, 0, 0)
                        );
                        Dictionary<int, Player.Clothing.Item> inventory = new Dictionary<int, Player.Clothing.Item>();
                        foreach(Data.Models.Item item in t.Result.Inventory)
                            inventory.Add(item.Id, items[item.Id]);
                        return new Loaded(player, inventory.ToImmutableDictionary());
                    }
                }
            ).PipeTo(Self); // Sends the result of the async task to self.
        }

        /// <summary>
        /// Initial running state/behaviour.
        /// </summary>
		protected override void Running() {
            Command<Packets.IReceivePacket>(new Action<Packets.IReceivePacket>(StashIncoming));
            Command<Loaded>(new Action<Loaded>(Loaded));
            // Recovery handlers.
            Recover<Loaded>(new Action<Loaded>(l => BecomeStacked(Ready)));
            Recover<Rooms.JoinRoom>(new Action<Rooms.JoinRoom>(jr => BecomeStacked(Transitioning)));
            Recover<SnapshotOffer>(offer => {
                state = (GameUserState) offer.Snapshot;
                BecomeStacked(State.Room == null ? new Action(Ready) : new Action(Joined));
            });
		}

        /// <summary>
        /// Loaded the user's player.
        /// </summary>
        /// <param name="player">Player.</param>
        private void Loaded(Loaded l) {
            if(l != null) {
                PersistAsync<Loaded>(l, loaded => {
                    BecomeStacked(Ready);
                    State = l.State;
                    Stash.UnstashAll();
                });
            }else{
                Connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.NO_DB_CONNECTION, -1), Self);
            }
        }

        private void Base() {
            Command<SaveSnapshotSuccess>(success => {
                DeleteMessages(success.Metadata.SequenceNr);
                if(success.Metadata.SequenceNr > 0) DeleteSnapshots(new SnapshotSelectionCriteria(success.Metadata.SequenceNr - 1));
            });
        }

        /// <summary>
        /// Ready state/behaviour (Become after loading player).
        /// </summary>
        private void Ready() {
            base.Running();
            Base();
            Command<Packets.Xt.Receive.Authentication.JoinServer>(new Action<Packets.Xt.Receive.Authentication.JoinServer>(JoinServer));
            Command<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(new Action<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(GetBuddies));
            Command<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(new Action<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(GetIgnored));
            Command<Packets.Xt.Receive.Player.Inventory.GetInventory>(new Action<Packets.Xt.Receive.Player.Inventory.GetInventory>(GetInventory));
            Command<Packets.Xt.Receive.GetLastRevision>(new Action<Packets.Xt.Receive.GetLastRevision>(GetLastRevision));
            Command<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(new Action<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(GetEPFPoints));
            Command<Packets.Xt.Receive.Heartbeat>(new Action<Packets.Xt.Receive.Heartbeat>(Heartbeat));
            Command<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
        }

        /// <summary>
        /// Joined state/behaviour (Become after joining the starting Room).
        /// </summary>
        private void Joined() {
            base.Running();
            Base();
            Command<Packets.Xt.Receive.Heartbeat>(new Action<Packets.Xt.Receive.Heartbeat>(Heartbeat));
            Command<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
            Command<Packets.Xt.Receive.Rooms.JoinRoom>(new Action<Packets.Xt.Receive.Rooms.JoinRoom>(JoinRoom));
            Command<Packets.Xt.Receive.Player.Move>(new Action<Packets.Xt.Receive.Player.Move>(Move));
            Command<Packets.Xt.Receive.Player.Say>(new Action<Packets.Xt.Receive.Player.Say>(s => Room.ActorRef.Tell(new Packets.Xt.Send.Player.Say(Player, s.Message, Room.InternalId), Self)));
            Command<Packets.Xt.Receive.Player.Action>(new Action<Packets.Xt.Receive.Player.Action>(a => Room.ActorRef.Tell(new Packets.Xt.Send.Player.Action(Player, a.Id, Room.InternalId), Self)));
            Command<Packets.Xt.Receive.Player.Frame>(new Action<Packets.Xt.Receive.Player.Frame>(Frame));
        }

        /// <summary>
        /// Transition state/behaviour (Become while joining another Room).
        /// </summary>
        private void Transitioning() {
            base.Running();
            Command<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
            Command<Packets.IReceivePacket>(new Action<Packets.IReceivePacket>(StashIncoming));
        }

        /// <summary>
        /// Stashes the incoming packets.
        /// </summary>
        /// <param name="received">Received packet.</param>
        private void StashIncoming(Packets.IReceivePacket received) {
            Stash.Stash();
        }

        /// <summary>
        /// Join server handler.
        /// </summary>
        /// <param name="js">Join server packet.</param>
		private void JoinServer(Packets.Xt.Receive.Authentication.JoinServer js) {
			// We'll check the login key again I GUESS
			// But for now let's just accept it.
			Connection.ActorRef.Tell(
				new Packets.Xt.Send.Authentication.JoinServer(
					agent: true, 
					guide: true,
					moderator: false, 
					modifiedStampCover: true
				)
			, Self);
		}

        /// <summary>
        /// Get buddies handler.
        /// </summary>
        /// <param name="gb">Get buddy packet.</param>
        private void GetBuddies(Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies gb) {
            Connection.ActorRef.Tell(new Packets.Xt.Send.Player.Relations.Buddies.GetBuddies(), Self); // TODO - Load actual buddy list.
        }

        /// <summary>
        /// Get ignore handler.
        /// </summary>
        /// <param name="gn">Get ignore packet.</param>
        private void GetIgnored(Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored gn) {
            Connection.ActorRef.Tell(new Packets.Xt.Send.Player.Relations.Ignore.GetIgnored(), Self); // TODO - Load actual ignore list.
        }

        /// <summary>
        /// Get inventory handler.
        /// </summary>
        /// <param name="gi">Get inventory packet.</param>
        private void GetInventory(Packets.Xt.Receive.Player.Inventory.GetInventory gi) {
            Connection.ActorRef.Tell(new Packets.Xt.Send.Player.Inventory.GetInventory(Inventory), Self); // TODO - Load actual inventory list.
        }

        /// <summary>
        /// Get last revision handler.
        /// </summary>
        /// <param name="glr">Get last revision packet.</param>
        private void GetLastRevision(Packets.Xt.Receive.GetLastRevision glr) {
            Connection.ActorRef.Tell(new Packets.Xt.Send.GetLastRevision(3239), Self); // Who knows where 3239 from.
            // Start joining a Room.
            BecomeStacked(Transitioning);
            // Join a random start Room
            user.Zone.ActorRef.Tell(new Rooms.JoinRoom(starts[(new Random()).Next(0, starts.Length)], Player), Self);
        }

        /// <summary>
        /// Get EPF points handler.
        /// </summary>
        /// <param name="epfgr">Get EPF points packet.</param>
        private void GetEPFPoints(Packets.Xt.Receive.Player.EPF.GetEPFPoints epfgr) {
            Connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.EPF.GetEPFPoints(0, 1) // TODO - Find out what these are?
            , Self);
        }

        /// <summary>
        /// Heartbeat handler.
        /// </summary>
        /// <param name="h">Heartbeat packet.</param>
        private void Heartbeat(Packets.Xt.Receive.Heartbeat h) {
            Connection.ActorRef.Tell(
                new Packets.Xt.Send.Heartbeat(h.Room)
            , Self);
        }

        /// <summary>
        /// Joined Room handler. Called when a joining a Room was successful.
        /// </summary>
        /// <param name="jr">Joined Room message.</param>
        private void JoinedRoom(JoinedRoom jr) {
            PersistAsync<JoinedRoom>(jr, joined => {
                if(Room != null) { // If we were in a Room before
                    Room.ActorRef.Tell(new Rooms.LeaveRoom(Player), Self); // Tell the Room we left.
                    UnbecomeStacked(); // Go back to regular state/behaviour.
                }else{ // If we weren't in a Room before
                    Become(Joined); // Become the Joined state/behaviour.
                }
                State = new GameUserState(jr.Player, Inventory, jr.Room);
                // Tell the client.
                Connection.ActorRef.Tell(new Packets.Xt.Send.Rooms.JoinedRoom(jr.Room.ExternalId, jr.Players, jr.Room.InternalId), Self);
                Stash.UnstashAll(); // Unstash any packets received in the meantime (should usually be none)
            });
        }

        /// <summary>
        /// Room full handler.
        /// </summary>
        /// <param name="rf">Room full message.</param>
        private void RoomFull(RoomFull rf) {
            if(State.Room != null) {
                Connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.ROOM_FULL, rf.Room.InternalId));
                UnbecomeStacked();
            }else{
                // Join a random start Room... again.
                user.Zone.ActorRef.Tell(new Rooms.JoinRoom(starts[(new Random()).Next(0, starts.Length)], Player), Self);
            }
        }

        /// <summary>
        /// Join Room handler.
        /// </summary>
        /// <param name="jr">Join Room packet.</param>
        private void JoinRoom(Packets.Xt.Receive.Rooms.JoinRoom jr) {
            Player.Player p = Player.UpdatePosition(new Player.Position(jr.X, jr.Y, 0)); // Set the player's position to that specified in the join Room packet.
            Rooms.JoinRoom request = new Rooms.JoinRoom(jr.ExternalId, p);
            user.Zone.ActorRef.Tell(request, Self); // Request to join the Room.
            Persist<Rooms.JoinRoom>(request, @join => {
                BecomeStacked(Transitioning); // Set the transistioning state.
            });
        }

        private void Move(Packets.Xt.Receive.Player.Move m) {
            State = State.UpdatePlayer(Player.UpdatePosition(Player.Position.UpdateCoordinates(m.X, m.Y)));
            Room.ActorRef.Tell(new Rooms.Move(State.Player), Self);
        }

        private void Frame(Packets.Xt.Receive.Player.Frame f) {
            State = State.UpdatePlayer(Player.UpdatePosition(Player.Position.UpdateFrame(f.Id)));
            Room.ActorRef.Tell(new Rooms.Frame(State.Player));
        }

        protected override void PostStop() {
            DeleteSnapshots(new SnapshotSelectionCriteria(LastSequenceNr));
        }
	}

    internal class Loaded {
        public GameUserState State {
            get;
        }

        public Loaded(Player.Player player, ImmutableDictionary<int, Player.Clothing.Item> inventory) {
            State = new GameUserState(player, inventory, null);
        }
    }

    /// <summary>
    /// Joined Room notification.
    /// </summary>
    internal class JoinedRoom {
        /// <summary>
        /// Gets the Room that the player joined.
        /// </summary>
        /// <value>The Room.</value>
        public Rooms.Room Room {
            get;
            private set;
        }

        /// <summary>
        /// Gets the player instance that the Room holds for the user.
        /// </summary>
        /// <value>The player instance.</value>
        public Player.Player Player {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Room's player list.
        /// </summary>
        /// <value>The Room's player list.</value>
        public ImmutableList<Player.Player> Players {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.JoinedRoom"/> class.
        /// </summary>
        /// <param name="room">Room that was joined.</param>
        /// <param name="player">Player instance that the Room has.</param>
        /// <param name="players">List of players in the Room.</param>
        public JoinedRoom(Rooms.Room room, Player.Player player, ImmutableList<Player.Player> players) {
            Room = room;
            Player = player;
            Players = players;
        }
    }

    /// <summary>
    /// Room full message.
    /// </summary>
    internal class RoomFull {
        /// <summary>
        /// Gets the Room that the user attempted to join.
        /// </summary>
        /// <value>The Room.</value>
        public Rooms.Room Room {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.RoomFull"/> class.
        /// </summary>
        /// <param name="room">The Room the player attempted to join.</param>
        public RoomFull(Rooms.Room room) {
            Room = room;
        }
    }
}

