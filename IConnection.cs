using System;
using System.Net;

namespace Snowbull.API {
	public interface IConnection : Observer.IObservable {
		EndPoint Address {
			get;
		}
	}
}

