using System;
using System.Collections.Immutable;
namespace Snowbull.API.Events.Notifications {
	public class Registered : Event {
		public Observer.IObservable Observable {
			get;
			private set;
		}

		public ImmutableArray<Observer.IObservable> Children {
			get;
			private set;
		}

		public Registered(Observer.IObservable observable, Observer.IObservable[] children) {
			Observable = observable;
			Children = children.ToImmutableArray();
		}
	}
}

