/**
 * Observable Event Poll for Snowbull's Plugin API ("Snowbull.API").
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
using Akka.Actor;

namespace Snowbull.API.Observer {
	internal sealed class Poll {
		private List<IActorRef> voters;
		private List<CancellableEventResponse> votes = new List<CancellableEventResponse>();

		public Events.CancellableEvent Event {
			get;
			private set;
		}

		public Poll(Events.CancellableEvent e, IActorRef[] polling) {
			Event = e;
			voters = new List<IActorRef>(polling);
		}

		public bool Vote(CancellableEventResponse response) {
			votes.Add(response);
			return voters.Count == votes.Count;
		}

		public bool Left(IActorRef voter) {
			voters.Remove(voter);
			return voters.Count == votes.Count;
		}

		public Consensus Consensus() {
			if(voters.Count == votes.Count) {
				bool cancelled = false;
				foreach(CancellableEventResponse response in votes)
					if(response.Cancelled == true)
						cancelled = true;
				return new Consensus(Event, cancelled);
			}else{
				return null;
			}
		}
	}
}

