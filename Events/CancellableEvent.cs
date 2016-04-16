using System;

namespace Snowbull.API.Events {
    public abstract class CancellableEvent : Event, ICancellableEvent {
        public bool Cancelled {
            get;
            internal set;
        }

        protected CancellableEvent(string name) : base(name) {
        }

        protected CancellableEvent() : base() {}
    }
}

