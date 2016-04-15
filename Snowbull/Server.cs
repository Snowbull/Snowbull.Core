using System;
using Akka.Actor;

namespace Snowbull {
	class Server : API.Observer.Observable, API.IServer {

		public Server(string name, IActorContext context) : base(name, context, null) {
		}
	}
}

