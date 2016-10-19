using System;
using Akka.Actor;
namespace Snowbull.Core {
    public interface IContext {
        IActorRef ActorRef {
            get;
        }
    }
}

