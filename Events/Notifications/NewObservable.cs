using System;

namespace Snowbull.API.Events.Notifications {
	public class NewObservable {
		public Observer.IObservable Observable {
			get;
			private set;
		}

		public NewObservable(Observer.IObservable observable) {
			Observable = observable;
		}
	}
}

