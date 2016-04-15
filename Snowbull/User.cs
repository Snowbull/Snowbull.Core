using System;
using Akka.Actor;

namespace Snowbull {
	class User : API.Observer.Observable, API.IUser {
		public User(string name, IActorContext context, IActorRef parent) : base(name, context, parent) {
		}
	}
}

