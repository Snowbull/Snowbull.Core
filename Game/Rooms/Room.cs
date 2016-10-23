/**
 * Immutable room context for Snowbull.
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

namespace Snowbull.Core.Game.Rooms {
    /// <summary>
    /// Immutable room context.
    /// </summary>
    public class Room : IRoom {
        /// <summary>
        /// Gets the room's actor reference.
        /// </summary>
        /// <value>The room's actor reference.</value>
        public IActorRef ActorRef {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room's internal identifier.
        /// </summary>
        /// <value>The room's internal identifier.</value>
        public int InternalID {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room's external identifier.
        /// </summary>
        /// <value>The room's external identifier.</value>
        public int ExternalID {
            get;
            private set;
        }

        /// <summary>
        /// Gets the room's name.
        /// </summary>
        /// <value>The room's name.</value>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the zone that the room is in.
        /// </summary>
        /// <value>The zone that the room is in.</value>
        public IZone Zone {
            get;
            private set;
        }

        protected Room(int internalId, int externalId, string name, IZone zone, int capacity, IActorContext c, Func<Room, Akka.Actor.Props> creator) {
            InternalID = internalId;
            ExternalID = externalId;
            Name = name;
            Zone = zone;
            ActorRef = c.ActorOf(creator(this), string.Format("room(Zone={0},Id={1},Name={2})", zone.Name, InternalID, Name));
        }

        public Room(int internalId, int externalId, string name, IZone zone, int capacity, IActorContext c) : this(internalId, externalId, name, zone, capacity, c, r => Akka.Actor.Props.Create(() => new RoomActor(r, capacity))) {
        }
    }
}

