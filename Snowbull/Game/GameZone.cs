using System;
using Akka.Actor;

namespace Snowbull.Game {
    public class GameZone : Zone {
        public GameZone(IActorRef server) : base(server) {
        }

        protected override void Authenticate(Authenticate authenticate) {

        }
    }
}

