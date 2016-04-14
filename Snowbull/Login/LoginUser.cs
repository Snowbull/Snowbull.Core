using System;
using Akka.Actor;

namespace Snowbull.Login {
	class LoginUser : User, API.Login.ILoginUser {
		public LoginUser(string name, IActorContext context) : base(name, context) {
		}
	}
}

