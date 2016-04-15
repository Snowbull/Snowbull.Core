using System;
using Akka.Actor;

namespace Snowbull.Game {
    sealed class GameZoneActor : ZoneActor {

		public GameZoneActor(string name, IActorRef server, IActorRef oparent) : base(name, (n, a) => new GameZone(n, a, oparent), server) {

        }

		protected override void Authenticate(Authenticate authenticate) {

        }
    }
}

