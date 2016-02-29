using System;

namespace Snowbull.API {
	public interface IObservable {
		string Name {
			get;
		}

		void Register(IObserver observer);
	}
}

