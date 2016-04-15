using System;
using Akka.Actor;

namespace Snowbull.API.Observer {
	internal abstract class Observable : IObservable {
		public string Name {
			get;
			private set;
		}

		internal IActorRef Actor {
			get;
			set;
		}

		public Observable(string name, IActorContext context, IActorRef parent) {
			Name = name;
			Actor = context.ActorOf(API.Observer.ObservableActor.Props(this, parent));
		}

		public void Send(Packets.ISendPacket packet) {
			Actor.Tell(packet);
		}

		internal void Notify(Events.Event e) {
			Actor.Tell(e);
		}
	}
}

