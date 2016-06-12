/**
 * Base Observer for Snowbull's Plugin API ("Snowbull.API").
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
using System.Collections.Generic;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;

namespace Snowbull.API.Observer {
	public abstract class Observer : IObserver {
		private Events.IEvent handling;
		private List<IObservable> observing = new List<IObservable>();
		private bool cancelled = false;

		internal IActorRef Actor {
			get;
			private set;
		}

		protected ImmutableArray<IObservable> Observing {
			get {
				return observing.ToImmutableArray();
			}
		}

		protected ILoggingAdapter Logger {
			get;
			private set;
		}

		public Observer(Context context) {
			Actor = context.Actor;
			Logger = context.Logger;
		}

		public bool Notify(IObservable source, Events.ICancellableEvent e) {
			handling = e;
			cancelled = false;
			Notified(source, (Events.ICancellableEvent) e);
			handling = null;
			return cancelled;
		}

		public void Notify(IObservable source, Events.IEvent e) {
			handling = e;
			Notified(source, e);
			handling = null;
		}

		protected void Cancel() {
			cancelled = true;
		}

		protected abstract void Notified(IObservable source, Events.IEvent e);

		protected abstract bool Notified(IObservable source, Events.ICancellableEvent e);

		protected void Observe(IObservable observable) {
			(observable as Observable).ObservableActor.Tell(new RegisterObserver(Actor));
			observing.Add(observable);
		}
	}
}

