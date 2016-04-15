using System;
using Akka.Actor;
using Akka.Event;

namespace Snowbull {
	internal abstract class SnowbullActor : ReceiveActor {
		protected readonly API.Observer.Observable Observable;
		protected readonly ILoggingAdapter Logger;

		public SnowbullActor(API.Observer.Observable observable) {
			Observable = observable;
			Logger = Logging.GetLogger(Context);
		}
	}
}

