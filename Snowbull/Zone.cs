using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Akka.Actor;

namespace Snowbull {
    public abstract class Zone {
        private readonly List<IActorRef> users = new List<IActorRef>();

        protected IReadOnlyList<IActorRef> Users {
            get {
                return users.AsReadOnly();
            }
        }

        protected Zone() {
        }
    }
}

