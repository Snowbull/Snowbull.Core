using System;
using Akka.Actor;

namespace Snowbull.Core.Game.Room {
    public class Room : IRoom {
        public IActorRef ActorRef {
            get;
            protected set;
        }

        public int InternalID {
            get;
            private set;
        }

        public int ExternalID {
            get;
            private set;
        }

        public string Name {
            get;
            private set;
        }

        public IZone Zone {
            get;
            private set;
        }

        public Room(int internalId, int externalId, string name, IZone zone, int capacity) {
            InternalID = internalId;
            ExternalID = externalId;
            Name = name;
            Zone = zone;
        }
    }
}

