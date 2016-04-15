using System;
using Akka.Actor;

namespace Snowbull.API {
	public sealed class Context {
		internal IActorRef Actor {
			get;
			private set;
		}

		public IServer Server {
			get;
			private set;
		}

		internal Akka.Event.ILoggingAdapter Logger {
			get;
			private set;
		}

		internal Context(IActorRef actor, IServer server, Akka.Event.ILoggingAdapter logger) {
			Actor = actor;
			Server = server;
			Logger = logger;
		}
	}
}

