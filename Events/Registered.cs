using System;

namespace Snowbull.API.Events {
	public class Registered : Event {
		public Observer.IObservable To {
			get;
			private set;
		}

		public Registered(Observer.IObservable observable) {
			To = observable;
		}
	}
}

