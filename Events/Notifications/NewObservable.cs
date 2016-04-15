using System;

namespace Snowbull.API.Events.Notifications {
	public class NewObservable : Event {
		public Observer.IObservable Parent {
			get;
			private set;
		}

		public Observer.IObservable Observable {
			get;
			private set;
		}

		public NewObservable(Observer.IObservable parent, Observer.IObservable observable) {
			Parent = parent;
			Observable = observable;
		}
	}
}

