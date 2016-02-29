using System;
using Akka.Actor;

namespace Snowbull {
	class Server : API.Observer.Observable, API.IServer {

		public Server(string name, IActorRef actor) : base(name, actor) {
		}
	}
}

