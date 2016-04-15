using System;

namespace Snowbull.API.Observer {
	public class Consensus {
		public Events.CancellableEvent Event {
			get;
			private set;
		}

		public bool Cancelled {
			get;
			private set;
		}

		public Consensus(Events.CancellableEvent e, bool cancelled) {
			Event = e;
			Cancelled = cancelled;
		}
	}
}

