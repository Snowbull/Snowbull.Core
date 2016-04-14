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

		private void HandleNotification(Notification n) {
			if(n.Event is Events.ICancellableEvent)
				Sender.Tell(new CancellableEventResponse(observer.Notify(n.Source, (Events.ICancellableEvent) n.Event)));
			else
				observer.Notify(n.Source, n.Event);
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

	internal class Notification {
		public IObservable Source {
			get;
			private set;
		}

		public Events.IEvent Event {
			get;
			private set;
		}

		public Notification(IObservable source, Events.IEvent e) {
			Source = source;
			Event = e;
		}
	}
}

