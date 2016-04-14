
using System;
using Akka.Actor;

namespace Snowbull {
	class Zone : API.Observer.Observable, API.IZone {
		public Zone(string name, IActorContext context) : base(name, context) {
		}
	}
}

