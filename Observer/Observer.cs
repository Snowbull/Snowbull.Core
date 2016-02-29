using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;

namespace Snowbull.API.Observer {
	public abstract class Observer {
		private Events.IEvent handling;
		private List<IObservable> observing = new List<IObservable>();

		internal IActorRef Actor {
			get;
			private set;
		}

		protected ImmutableArray<IObservable> Observing {
			get {
				return observing.ToImmutableArray();
			}
		}

		public void Notify(IObservable source, Events.IEvent e) {
			handling = e;
			Notified(source, e);
			handling = null;
		}

		protected abstract void Notified(IObservable source, Events.IEvent e);

		protected void Observe(Observable observable) {
			observable.Actor.Tell(new RegisterObserver(this, Actor));
			observing.Add(observable);
		}
	}
}

