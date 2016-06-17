using System;
using Akka.Actor;

namespace Snowbull.Game {
	internal class GameUserActor : UserActor {
		public static Props Props(GameUser user) {
			return Akka.Actor.Props.Create(() => new GameUserActor(user));
		}

		public GameUserActor(GameUser user) : base(user) {
		}
	}
}

