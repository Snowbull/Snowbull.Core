using System;
using Akka.Actor;

namespace Snowbull {
	class User : API.Observer.Observable, API.IUser {
		public User(string name, IActorContext context) : base(name, context) {
		}
	}
}

