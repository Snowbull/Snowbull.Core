using System;
using Akka.Actor;
using System.Collections.Generic;

namespace Snowbull.API.Observer {
	internal class ObservableActor : ReceiveActor {
		private List<Observer> observers = new List<Observer>();

		protected Observable Observable {
			get;
			private set;
		}

		public static Props Props(Observable observable) {
			return Akka.Actor.Props.Create(() => new ObservableActor(observable));
		}

		public ObservableActor(Observable observable) {
			Observable = observable;
			Receive<Events.IEvent>(Event);
			Receive<RegisterObserver>(RegisterObserver);
		}

		private void RegisterObserver(RegisterObserver ro) {
			observers.Add(ro.Observer);
		}

		private void Event(Events.IEvent e) {
			foreach(Observer observer in observers)
				observer.Actor.Tell(new Notification(Observable, e), Self);
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

	public class CancellableEventResponse {
		public bool Cancelled {
			get;
			private set;
		}

		public CancellableEventResponse(bool cancelled) {
			Cancelled = cancelled;
		}
	}
}

