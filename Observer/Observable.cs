using System;
using Akka.Actor;

namespace Snowbull.API.Observer {
	public abstract class Observable : IObservable {
		public string Name {
			get;
			private set;
		}

		internal IActorRef Actor {
			get;
			private set;
		}

		public Observable(string name, IActorRef actor) {
			Name = name;
			Actor = actor;
		}

		public void Send(Packets.ISendPacket packet) {
			Actor.Tell(packet);
		}
	}
}

