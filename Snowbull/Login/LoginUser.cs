using System;
using Akka.Actor;

namespace Snowbull.Login {
	class LoginUser : User, API.Login.ILoginUser {
		public LoginUser(string name, IActorRef actor) : base(name, actor) {
		}
	}
}

