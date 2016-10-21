using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull.Core.Game.Room {
    public class RoomActor : SnowbullActor {
        protected readonly Room room;
        protected readonly List<User> users = new List<User>();
        protected readonly int capacity;


        public RoomActor(Room room, int capacity) {
            this.room = room;
            this.capacity = capacity;
        }
    }
}

