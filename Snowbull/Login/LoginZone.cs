using System;
using Akka.Actor;

namespace Snowbull.Login {
	class LoginZone : Zone, API.Login.ILoginZone {
		public LoginZone(string name, IActorRef actor) : base(name, actor) {
		}
	}
}

