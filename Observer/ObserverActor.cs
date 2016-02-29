using System;
using Akka.Actor;

namespace Snowbull.API.Observer {
	internal class ObserverActor : ReceiveActor {
		private Observer observer;

		public static Props Props(Observer observer) {
			return Akka.Actor.Props.Create(() => new ObserverActor(observer));
		}

		public ObserverActor(Observer observer) {
			this.observer = observer;
		}

		private void Observe(Observe observe) {
			Observable observable = (observe.Observable as Observable);
			if(observable != null)
				observable.Actor.Tell(new RegisterObserver((IObserver) observer));
		}

		private void HandleEvent(EventHappened e) {
			observer.Notify(e.Source, e.Event);
		}
	}

	internal class Observe {
		public IObservable Observable {
			get;
			private set;
		}

		public Observe(IObservable observable) {
			Observable = observable;
		}
	}

	internal class EventHappened {
		public IObservable Source {
			get;
			private set;
		}

		public Events.IEvent Event {
			get;
			private set;
		}

		public EventHappened(IObservable source, Events.IEvent e) {
			Source = source;
			Event = e;
		}
	}

	internal class RegisterObserver {
		public IObserver Observer {
			get;
			private set;
		}

		public RegisterObserver(IObserver observer) {
			Observer = observer;
		}
	}
}

