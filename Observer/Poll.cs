using System;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull.API.Observer {
	internal class Poll {
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

