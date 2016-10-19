using System;
using Akka.Actor;

namespace Snowbull.Core {
    public class Room : IRoom {
        public IActorRef ActorRef {
            get;
            protected set;
        }

        public Room() {
        }
    }
}

