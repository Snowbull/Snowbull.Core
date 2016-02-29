using System;
using Akka.Actor;

namespace Snowbull.Game {
	class GameZone : Zone, API.Game.IGameZone {
		public GameZone(string name, IActorRef actor) : base(name, actor) {
		}
	}
}

