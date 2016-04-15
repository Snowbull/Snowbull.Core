using System;
using System.Net;
using Akka.Actor;

namespace Snowbull {
	internal class Connection : API.Observer.Observable, API.IConnection {
		public EndPoint Address {
			get;
			private set;
		}

		public Connection(EndPoint address, IActorContext context, IActorRef parent) : base(address.ToString(), context, parent) {
			Address = address;
		}
	}
}

