/**
 * Observer Akka Actor for Snowbull's Plugin API ("Snowbull.API").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull.API".
 * 
 * "Snowbull.API" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull.API" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull.API". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;
using Akka.Actor;
using Akka.Event;

namespace Snowbull.API.Observer {
	internal sealed class ObserverActor : ReceiveActor {
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

