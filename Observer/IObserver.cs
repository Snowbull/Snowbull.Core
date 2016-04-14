using System;

namespace Snowbull.API.Observer {
	public interface IObserver {
		void Notify(IObservable source, Events.IEvent e);

		bool Notify(IObservable source, Events.ICancellableEvent e);
	}
}

