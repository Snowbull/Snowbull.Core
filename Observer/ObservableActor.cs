using System;
using Akka.Actor;
using System.Collections.Generic;

namespace Snowbull.API.Observer {
	internal class ObservableActor : ReceiveActor {
		private Dictionary<Observer, IActorRef> observers;

		protected Observable Observable {
			get;
			private set;
		}

		public ObservableActor(string name, Func<string, IActorRef, Observable> creator) {
			Observable = creator(name, Self);
		}

		protected void NotifyAll(Events.IEvent e) {
			foreach(IActorRef observer in observers.Values)
				observer.Tell(new EventHappened(Observable, e), Self);
		}

	}
}

