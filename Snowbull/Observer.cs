using System;
using Akka.Actor;

namespace Snowbull {
	public class Observer : ReceiveActor {
		private API.IObserver inner;

		public static Props Props(API.IObserver inner) {
			return Akka.Actor.Props.Create(() => new Observer(inner));
		}

		public Observer(API.Observer inner) {
			this.inner = inner;
		}

		private void HandleEvent(EventHappened e) {
			inner.Notify(e.Source, e.Event);
		}
	}

	public class EventHappened {
		public API.IObservable Source {
			get;
			private set;
		}

		public API.Events.IEvent Event {
			get;
			private set;
		}

		public EventHappened(API.IObservable source, API.Events.IEvent e) {
			Source = source;
			Event = e;
		}
	}
}

