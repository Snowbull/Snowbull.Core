using System;
using Akka.Actor;

namespace Snowbull.Core.Game {
	public class GameUser : User {
		public GameUser(int id, string username, IActorContext c, Connection connection, Zone zone) : base(id, username, connection, zone) {
			ActorRef = c.ActorOf(GameUserActor.Props(this), string.Format("user(Id={0},Username={1}", id, username));
			connection.ActorRef.Tell(new Authenticated(this), ActorRef);
			connection.ActorRef.Tell(new Packets.Xt.Send.Authentication.Login(), ActorRef);
		}
	}
}

