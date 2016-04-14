using System;
using Akka.Actor;

namespace Snowbull.Login {
	class LoginZone : Zone, API.Login.ILoginZone {
		public LoginZone(string name, IActorContext context) : base(name, context) {
		}
	}
}

