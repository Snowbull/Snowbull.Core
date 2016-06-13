using System;
using Akka.Actor;

namespace Snowbull {
	internal interface IContext {
		IActorRef ActorRef {
			get;
		}
	}
}

