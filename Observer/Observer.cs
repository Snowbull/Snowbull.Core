using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;

namespace Snowbull.API.Observer {
	public abstract class Observer : IObserver {
		private Events.IEvent handling;
		private List<IObservable> observing = new List<IObservable>();
		private bool cancelled = false;

		internal IActorRef Actor {
			get;
			private set;
		}

		protected ImmutableArray<IObservable> Observing {
			get {
				return observing.ToImmutableArray();
			}
		}

		public bool Notify(IObservable source, Events.ICancellableEvent e) {
			handling = e;
			cancelled = false;
			Notified(source, (Events.ICancellableEvent) e);
			handling = null;
			return cancelled;
		}

		public void Notify(IObservable source, Events.IEvent e) {
			handling = e;
			Notified(source, e);
			handling = null;
		}

		protected void Cancel() {
			cancelled = true;
		}

		protected abstract void Notified(IObservable source, Events.IEvent e);

		protected abstract bool Notified(IObservable source, Events.ICancellableEvent e);

		protected void Observe(Observable observable) {
			observable.Actor.Tell(new RegisterObserver(this));
			observing.Add(observable);
		}
	}
}

