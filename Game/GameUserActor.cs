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
using System.Data.Entity;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Snowbull.Core.Game {
    public class GameUserActor : UserActor, IWithUnboundedStash {
        private Player.Player player;
        private Rooms.Room room = null;
        private static readonly ImmutableArray<int> starts = (new int[] {100, 200, 300, 400, 800, 801, 802, 230, 810, 804}).ToImmutableArray(); // Starting rooms.
        private ImmutableDictionary<int, Player.Clothing.Item> items;
        private readonly Dictionary<int, Player.Clothing.Item> inventory = new Dictionary<int, Player.Clothing.Item>();

        /// <summary>
        /// Gets or sets the stash. This will be automatically populated by the framework AFTER the constructor has been run.
        ///  Implement this as an auto property.
        /// </summary>
        /// <value>The stash.</value>
        public IStash Stash {
            get;
            set;
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
        public GameUserActor(GameUser user, ImmutableDictionary<int, Player.Clothing.Item> items) : base(user) {
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
                        List<Player.Clothing.Item> inventory = new List<Player.Clothing.Item>();
                        foreach(Data.Models.Item item in t.Result.Inventory)
                            inventory.Add(items[item.Id]);
                        return new Loaded(player, inventory.ToImmutableList());
                    }
                }
            ).PipeTo(Self); // Sends the result of the async task to self.
        }

        /// <summary>
        /// Initial running state/behaviour.
        /// </summary>
		protected override void Running() {
            Receive<Packets.IReceivePacket>(new Action<Packets.IReceivePacket>(StashIncoming));
            Receive<Loaded>(new Action<Loaded>(Loaded));
		}

        /// <summary>
        /// Loaded the user's player.
        /// </summary>
        /// <param name="player">Player.</param>
        private void Loaded(Loaded l) {
            if(l != null) {
                this.player = l.Player;
                foreach(Player.Clothing.Item item in l.Inventory)
                    inventory.Add(item.Id, item);
                BecomeStacked(Ready);
                Stash.UnstashAll();
            }else{
                connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.NO_DB_CONNECTION, -1), Self);
            }
        }

        /// <summary>
        /// Ready state/behaviour (Become after loading player).
        /// </summary>
        private void Ready() {
            base.Running();
            Receive<Packets.Xt.Receive.Authentication.JoinServer>(new Action<Packets.Xt.Receive.Authentication.JoinServer>(JoinServer));
            Receive<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(new Action<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(GetBuddies));
            Receive<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(new Action<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(GetIgnored));
            Receive<Packets.Xt.Receive.Player.Inventory.GetInventory>(new Action<Packets.Xt.Receive.Player.Inventory.GetInventory>(GetInventory));
            Receive<Packets.Xt.Receive.GetLastRevision>(new Action<Packets.Xt.Receive.GetLastRevision>(GetLastRevision));
            Receive<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(new Action<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(GetEPFPoints));
            Receive<Packets.Xt.Receive.Heartbeat>(new Action<Packets.Xt.Receive.Heartbeat>(Heartbeat));
            Receive<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
        }

        /// <summary>
        /// Joined state/behaviour (Become after joining the starting Room).
        /// </summary>
        private void Joined() {
            base.Running();
            Receive<Packets.Xt.Receive.Heartbeat>(new Action<Packets.Xt.Receive.Heartbeat>(Heartbeat));
            Receive<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
            Receive<Packets.Xt.Receive.Rooms.JoinRoom>(new Action<Packets.Xt.Receive.Rooms.JoinRoom>(JoinRoom));
            Receive<Packets.Xt.Receive.Player.Move>(new Action<Packets.Xt.Receive.Player.Move>(Move));
            Receive<Packets.Xt.Receive.Player.Say>(new Action<Packets.Xt.Receive.Player.Say>(s => room.ActorRef.Tell(new Packets.Xt.Send.Player.Say(player, s.Message, room.InternalId), Self)));
            Receive<Packets.Xt.Receive.Player.Action>(new Action<Packets.Xt.Receive.Player.Action>(a => room.ActorRef.Tell(new Packets.Xt.Send.Player.Action(player, a.Id, room.InternalId), Self)));
            Receive<Packets.Xt.Receive.Player.Frame>(new Action<Packets.Xt.Receive.Player.Frame>(Frame));
        }

        /// <summary>
        /// Transition state/behaviour (Become while joining another Room).
        /// </summary>
        private void Transitioning() {
            base.Running();
            Receive<JoinedRoom>(new Action<JoinedRoom>(JoinedRoom));
            Receive<Packets.IReceivePacket>(new Action<Packets.IReceivePacket>(StashIncoming));
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
			connection.ActorRef.Tell(
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
            connection.ActorRef.Tell(new Packets.Xt.Send.Player.Relations.Buddies.GetBuddies(), Self); // TODO - Load actual buddy list.
        }

        /// <summary>
        /// Get ignore handler.
        /// </summary>
        /// <param name="gn">Get ignore packet.</param>
        private void GetIgnored(Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored gn) {
            connection.ActorRef.Tell(new Packets.Xt.Send.Player.Relations.Ignore.GetIgnored(), Self); // TODO - Load actual ignore list.
        }

        /// <summary>
        /// Get inventory handler.
        /// </summary>
        /// <param name="gi">Get inventory packet.</param>
        private void GetInventory(Packets.Xt.Receive.Player.Inventory.GetInventory gi) {
            connection.ActorRef.Tell(new Packets.Xt.Send.Player.Inventory.GetInventory(inventory), Self); // TODO - Load actual inventory list.
        }

        /// <summary>
        /// Get last revision handler.
        /// </summary>
        /// <param name="glr">Get last revision packet.</param>
        private void GetLastRevision(Packets.Xt.Receive.GetLastRevision glr) {
            connection.ActorRef.Tell(new Packets.Xt.Send.GetLastRevision(3239), Self); // Who knows where 3239 from.
            // Start joining a Room.
            BecomeStacked(Transitioning);
            // Join a random start Room
            user.Zone.ActorRef.Tell(new Rooms.JoinRoom(starts[(new Random()).Next(0, starts.Length)], player), Self);
        }

        /// <summary>
        /// Get EPF points handler.
        /// </summary>
        /// <param name="epfgr">Get EPF points packet.</param>
        private void GetEPFPoints(Packets.Xt.Receive.Player.EPF.GetEPFPoints epfgr) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.EPF.GetEPFPoints(0, 1) // TODO - Find out what these are?
            , Self);
        }

        /// <summary>
        /// Heartbeat handler.
        /// </summary>
        /// <param name="h">Heartbeat packet.</param>
        private void Heartbeat(Packets.Xt.Receive.Heartbeat h) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Heartbeat(h.Room)
            , Self);
        }

        /// <summary>
        /// Joined Room handler. Called when a joining a Room was successful.
        /// </summary>
        /// <param name="jr">Joined Room message.</param>
        private void JoinedRoom(JoinedRoom jr) {
            if(room != null) { // If we were in a Room before
                room.ActorRef.Tell(new Rooms.LeaveRoom(player), Self); // Tell the Room we left.
                UnbecomeStacked(); // Go back to regular state/behaviour.
            }else{ // If we weren't in a Room before
                Become(Joined); // Become the Joined state/behaviour.
            }
            room = jr.Room; // Set the new Room.
            player = jr.Player; // Set our player object to the one sent back by the Room, for consistency
            // Tell the client.
            connection.ActorRef.Tell(new Packets.Xt.Send.Rooms.JoinedRoom(jr.Room.ExternalId, jr.Players, jr.Room.InternalId), Self);
            Stash.UnstashAll(); // Unstash any packets received in the meantime (should usually be none)
        }

        /// <summary>
        /// Room full handler.
        /// </summary>
        /// <param name="rf">Room full message.</param>
        private void RoomFull(RoomFull rf) {
            if(room != null) {
                connection.ActorRef.Tell(new Packets.Xt.Send.Error(Errors.ROOM_FULL, rf.Room.InternalId));
            }else{
                // Join a random start Room... again.
                user.Zone.ActorRef.Tell(new Rooms.JoinRoom(starts[(new Random()).Next(0, starts.Length)], player), Self);
            }
        }

        /// <summary>
        /// Join Room handler.
        /// </summary>
        /// <param name="jr">Join Room packet.</param>
        private void JoinRoom(Packets.Xt.Receive.Rooms.JoinRoom jr) {
            BecomeStacked(Transitioning); // Set the transistioning state.
            Player.Player p = player.UpdatePosition(new Player.Position(jr.X, jr.Y, 0)); // Set the player's position to that specified in the join Room packet.
            user.Zone.ActorRef.Tell(new Rooms.JoinRoom(jr.ExternalId, p)); // Request to join the Room.
        }

        private void Move(Packets.Xt.Receive.Player.Move m) {
            player = player.UpdatePosition(player.Position.UpdateCoordinates(m.X, m.Y));
            room.ActorRef.Tell(new Rooms.Move(player), Self);
        }

        private void Frame(Packets.Xt.Receive.Player.Frame f) {
            player = player.UpdatePosition(player.Position.UpdateFrame(f.Id));
            room.ActorRef.Tell(new Rooms.Frame(player));
        }
	}

    internal class Loaded {
        public Player.Player Player {
            get;
            private set;
        }

        public ImmutableList<Player.Clothing.Item> Inventory {
            get;
            private set;
        }

        public Loaded(Player.Player player, ImmutableList<Player.Clothing.Item> inventory) {
            Player = player;
            Inventory = inventory;
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

