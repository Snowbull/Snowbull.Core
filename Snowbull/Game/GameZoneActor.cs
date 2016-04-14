using System;
using Akka.Actor;

namespace Snowbull.Game {
    sealed class GameZoneActor : ZoneActor {

		public GameZoneActor(string name, IActorRef server) : base(name, (n, a) => new GameZone(n, a), server) {

        }

		protected override void Authenticate(Authenticate authenticate) {

        }
    }
}

