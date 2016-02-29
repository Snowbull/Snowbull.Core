
using System;
using Akka.Actor;

namespace Snowbull {
	class Zone : API.Observer.Observable, API.IZone {
		public Zone(string name, IActorRef actor) : base(name, actor) {
		}
	}
}

