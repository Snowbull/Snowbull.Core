using System;
using Akka.Actor;

namespace Snowbull.Game {
	class GameZone : Zone, API.Game.IGameZone {
		public GameZone(string name, IActorContext context) : base(name, context) {
		}
	}
}

