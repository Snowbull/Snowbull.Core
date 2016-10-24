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
        protected readonly Room Room;
        private readonly Dictionary<IActorRef, Player.Player> players = new Dictionary<IActorRef, Player.Player>();
        protected int Capacity;

        protected ImmutableList<Player.Player> Players {
            get { return players.Values.ToImmutableList();  }
        }

        /// <summary>
        /// Gets the internal identifier.
        /// </summary>
        /// <value>The internal identifier.</value>
        protected int InternalId {
            get { return Room.InternalId; }
        }

        /// <summary>
        /// Gets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        protected int ExternalId {
            get { return Room.ExternalId; }
        }

        /// <summary>
        /// Gets the room's name.
        /// </summary>
        /// <value>The room's name.</value>
        protected string Name {
            get { return Room.Name; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Rooms.RoomActor"/> class.
        /// </summary>
        /// <param name="room">Immutable room context.</param>
        /// <param name="capacity">Room's capacity.</param>
        public RoomActor(Room room, int capacity) {
            this.Room = room;
            this.Capacity = capacity;
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
            Receive<Move>(new Action<Move>(Move));
            Receive<Frame>(new Action<Frame>(Frame));
        }

        /// <summary>
        /// Join room handler.
        /// </summary>
        /// <param name="jr">Join room request.</param>
        private void Join(JoinRoom jr) {
            if(players.Count < Capacity) {
                Add(jr.Player);
                Context.Watch(jr.Player.User.ActorRef); // Start watching user actor for termination.
            }else{
                Sender.Tell(new RoomFull(Room)); // Tell the user that the room is full, so they cannot join.
            }
        }

        /// <summary>
        /// Add the specified player.
        /// </summary>
        /// <param name="player">Player to add.</param>
        protected virtual void Add(Player.Player player) { // protected virtual so Game rooms can override it.
            Send(new Packets.Xt.Send.Rooms.AddPlayer(player.User.Id, player.ToString(), InternalId)); // Tell everyone a new player has joined the room.
            players.Add(player.User.ActorRef, player); // Add the player to the list.
            Sender.Tell(new JoinedRoom(Room, player, players.Values.ToImmutableList())); // Tell the user they have successfully joined the room.
        }

        /// <summary>
        /// Leave room handler.
        /// </summary>
        /// <param name="lr">Leave room notification.</param>
        private void Leave(LeaveRoom lr) {
            Remove(lr.Player.User.ActorRef);
        }

        /// <summary>
        /// Terminated actor handler. Listens for User actors stopping (so the player can be removed).
        /// </summary>
        /// <param name="t">Actor termination notification.</param>
        private void Terminated(Terminated t) {
            Remove(t.ActorRef);
        }

        /// <summary>
        /// Remove the specified player.
        /// </summary>
        /// <param name="actor">The actor of the player to remove.</param>
        private void Remove(IActorRef actor) {
            Player.Player player = players[actor];
            if(player != null) {
                players.Remove(actor); // Remove the player from the list.
                Send(new Packets.Xt.Send.Rooms.RemovePlayer(player.User.Id, InternalId));
                Context.Unwatch(player.User.ActorRef);
            }
        }

        private void Move(Move m) {
            players[m.Player.User.ActorRef] = m.Player;
            Send(new Packets.Xt.Send.Player.Move(m.Player, InternalId));
        }

        private void Frame(Frame f) {
            players[f.Player.User.ActorRef] = f.Player;
            Send(new Packets.Xt.Send.Player.Frame(f.Player, f.Player.Position.Frame, InternalId));
        }

        /// <summary>
        /// Send the specified packet to all players in the room.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        private void Send(Packets.ISendPacket packet) {
            foreach(Player.Player player in players.Values)
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
        public int ExternalId {
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
            ExternalId = externalId;
            Player = player;
        }
    }

    /// <summary>
    /// Leave room notification.
    /// </summary>
    public sealed class LeaveRoom {
        /// <summary>
        /// Gets the player to remove.
        /// </summary>
        /// <value>The player to remove.</value>
        public Player.Player Player {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Rooms.LeaveRoom"/> class.
        /// </summary>
        /// <param name="player">Leaving player.</param>
        public LeaveRoom(Player.Player player) {
            Player = player;
        }
    }

    public sealed class Move {
        public Player.Player Player {
            get;
            private set;
        }

        public Move(Player.Player player) {
            Player = player;
        }
    }

    public sealed class Frame {
        public Player.Player Player {
            get;
            private set;
        }

        public Frame(Player.Player player) {
            Player = player;
        }
    }
}

