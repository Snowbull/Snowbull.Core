using System;

namespace Snowbull.API {
	public interface IObserver {
		void Notify(IObservable source, Events.IEvent e);
	}
}

