using System;
using Akka.Actor;

namespace Snowbull.Game {
	internal class GameUserActor : UserActor {
		public static Props Props(GameUser user) {
			return Akka.Actor.Props.Create(() => new GameUserActor(user));
		}

		public GameUserActor(GameUser user) : base(user) {
		}

		protected override void Running() {
			base.Running();
			Receive<Packets.Xt.Receive.Authentication.JoinServer>(JoinServer);
		}

		private void JoinServer(Packets.Xt.Receive.Authentication.JoinServer js) {
			// We'll check the login key again I GUESS
			// But for now let's just accept it.
			connection.ActorRef.Tell(
				new Packets.Xt.Send.Authentication.JoinServer(
					agent: true, 
					guide: true,
					moderator: false, 
					modifiedStampCover: true
				)
			, Self);
		}
	}
}

