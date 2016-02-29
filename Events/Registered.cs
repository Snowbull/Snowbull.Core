using System;

namespace Snowbull.API.Events {
	public class Registered : Event {
		public IObservable To {
			get;
			private set;
		}

		public Registered(IObservable observable) {
			To = observable;
		}
	}
}

