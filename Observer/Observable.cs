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
			set;
		}

		public Observable(string name, IActorContext context) {
			Name = name;
			Actor = context.ActorOf(API.Observer.ObservableActor.Props(this));
		}

		public void Send(Packets.ISendPacket packet) {
			Actor.Tell(packet);
		}
	}
}

