using System;
using Akka.Actor;

namespace Snowbull.Core.Game.Rooms {
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

