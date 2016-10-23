/**
 * Room actor for Snowbull.
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
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;

namespace Snowbull.Core.Game.Rooms {
    /// <summary>
    /// Room actor.
    /// </summary>
    public class RoomActor : SnowbullActor {
        protected readonly Room room;
        protected readonly List<Player.Player> players = new List<Player.Player>();
        protected int capacity;

        /// <summary>
        /// Gets the internal identifier.
        /// </summary>
        /// <value>The internal identifier.</value>
        protected int InternalID {
            get { return room.InternalID; }
        }

        /// <summary>
        /// Gets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        protected int ExternalID {
            get { return room.ExternalID; }
        }

        /// <summary>
        /// Gets the room's name.
        /// </summary>
        /// <value>The room's name.</value>
        protected string Name {
            get { return room.Name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Rooms.RoomActor"/> class.
        /// </summary>
        /// <param name="room">Immutable room context.</param>
        /// <param name="capacity">Room's capacity.</param>
        public RoomActor(Room room, int capacity) {
            this.room = room;
            this.capacity = capacity;
            Become(Running); // Switch to running state.
        }

        /// <summary>
        /// Regular running behaviour/state.
        /// </summary>
        protected virtual void Running() {
            // Add handlers.
            Receive<JoinRoom>(new Action<JoinRoom>(Join));
            Receive<LeaveRoom>(new Action<LeaveRoom>(Leave));
            Receive<Packets.ISendPacket>(new Action<Packets.ISendPacket>(Send));
            Receive<Terminated>(new Action<Terminated>(Terminated));
        }

        /// <summary>
        /// Join room handler.
        /// </summary>
        /// <param name="jr">Join room request.</param>
        private void Join(JoinRoom jr) {
            if(players.Count < capacity) {
                Add(jr.Player);
                Context.Watch(jr.Player.User.ActorRef); // Start watching user actor for termination.
            }else{
                Sender.Tell(new RoomFull(room)); // Tell the user that the room is full, so they cannot join.
            }
        }

        /// <summary>
        /// Add the specified player.
        /// </summary>
        /// <param name="player">Player to add.</param>
        protected virtual void Add(Player.Player player) { // protected virtual so Game rooms can override it.
            Send(new Packets.Xt.Send.Rooms.AddPlayer(player.User.Id, player.ToString(), InternalID)); // Tell everyone a new player has joined the room.
            players.Add(player); // Add the player to the list.
            Sender.Tell(new JoinedRoom(room, player, players.ToImmutableList())); // Tell the user they have successfully joined the room.
        }

        /// <summary>
        /// Leave room handler.
        /// </summary>
        /// <param name="lr">Leave room notification.</param>
        private void Leave(LeaveRoom lr) {
            foreach(Player.Player player in players) {
                if(player.User == lr.User) { // If it's the player of the specified user...
                    Remove(player); // Remove player
                    break;
                }
            }
        }

        /// <summary>
        /// Terminated actor handler. Listens for User actors stopping (so the player can be removed).
        /// </summary>
        /// <param name="t">Actor termination notification.</param>
        private void Terminated(Terminated t) {
            foreach(Player.Player player in players) {
                if(player.User.ActorRef == t.ActorRef) { // If it's the player of the user of the specified actor...
                    Remove(player); // Remove the player.
                    break;
                }
            }
        }

        /// <summary>
        /// Remove the specified player.
        /// </summary>
        /// <param name="player">The player to remove.</param>
        private void Remove(Player.Player player) {
            players.Remove(player); // Remove the player from the list.
            Send(new Packets.Xt.Send.Rooms.RemovePlayer(player.User.Id, InternalID));
            Context.Unwatch(player.User.ActorRef);
        }

        /// <summary>
        /// Send the specified packet to all players in the room.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        private void Send(Packets.ISendPacket packet) {
            foreach(Player.Player player in players)
                player.User.ActorRef.Tell(packet, Self);
        }
    }

    /// <summary>
    /// Join room message.
    /// </summary>
    public sealed class JoinRoom {
        /// <summary>
        /// Gets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        public int ExternalID {
            get;
            private set;
        }

        /// <summary>
        /// Gets the player to add.
        /// </summary>
        /// <value>The player to add.</value>
        public Player.Player Player {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Rooms.JoinRoom"/> class.
        /// </summary>
        /// <param name="externalId">External identifier.</param>
        /// <param name="player">Player.</param>
        public JoinRoom(int externalId, Player.Player player) {
            ExternalID = externalId;
            Player = player;
        }
    }

    /// <summary>
    /// Leave room notification.
    /// </summary>
    public sealed class LeaveRoom {
        /// <summary>
        /// Gets the user to remove.
        /// </summary>
        /// <value>The user to remove.</value>
        public GameUser User { // It is safer to use User here because Player can change.
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Rooms.LeaveRoom"/> class.
        /// </summary>
        /// <param name="user">Leaving user.</param>
        public LeaveRoom(GameUser user) {
            User = user;
        }
    }
}

