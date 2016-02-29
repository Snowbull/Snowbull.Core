using System;

namespace Snowbull.API {
	public abstract class Observer {
		private Events.IEvent handling;

		internal bool Cancelled {
			get;
			private set;
		}

		protected IServer Server {
			get;
			private set;
		}

		public Observer(IServer server) {
			Server = server;
		}

		public void Notify(IObservable source, Events.IEvent e) {
			Cancelled = false;
			handling = e;
			Notified(source, e);
			handling = null;
		}

		protected abstract void Notified(IObservable source, Events.IEvent e);

		protected void Cancel() {
			if(!(handling is Events.ICancellableEvent))
				throw new InvalidOperationException("Only events implementing ICancellableEvent can be cancelled.");
			Cancelled = true;
		}
	}
}

