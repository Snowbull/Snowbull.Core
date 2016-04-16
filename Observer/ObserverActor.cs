using System;
using Akka.Actor;
using Akka.Event;

namespace Snowbull.API.Observer {
	internal class ObserverActor : ReceiveActor {
		private Observer observer;

		public static Props Props(IServer server, Func<Context, Observer> creator) {
			return Akka.Actor.Props.Create(() => new ObserverActor(server, creator));
		}

		public ObserverActor(IServer server, Func<Context, Observer> creator) {
			observer = creator(new Context(Self, server, Context.System.Log));
			Receive<Notification>(HandleNotification);
			Receive<Observe>(Observe);
		}

		private void Observe(Observe observe) {
			Observable observable = (observe.Observable as Observable);
			if(observable != null)
				observable.Actor.Tell(new RegisterObserver(Self));
		}

		private void HandleNotification(Notification n) {
			Events.CancellableEvent c = n.Event as Events.CancellableEvent;
			if(c != null)
				Sender.Tell(new CancellableEventResponse(c, observer.Notify(n.Source, c)));
			else
				observer.Notify(n.Source, n.Event);
		}
	}

	internal sealed class Observe {
		public IObservable Observable {
			get;
			private set;
		}

		public Observe(IObservable observable) {
			Observable = observable;
		}
	}

	internal sealed class Notification {
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

