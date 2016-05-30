/**
 * Base Observable for Snowbull's Plugin API ("Snowbull.API").
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

namespace Snowbull.API.Observer {
	internal abstract class Observable : IObservable {
		public string Name {
			get;
			private set;
		}

		internal IActorRef Actor {
			get;
			set;
		}

		public Observable(string name, IActorContext context, IActorRef parent) {
			Name = name;
			Actor = context.ActorOf(API.Observer.ObservableActor.Props(this, parent));
		}

		public void Send(Packets.ISendPacket packet) {
			Actor.Tell(packet);
		}

		internal void Notify(Events.Event e) {
			Actor.Tell(e);
		}
	}
}

