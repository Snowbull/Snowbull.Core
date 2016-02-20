using System;

namespace Snowbull.API.Events {
    public class CancellableEvent : Event, ICancellableEvent {
        public bool Cancelled {
            get;
            internal set;
        }

        protected CancellableEvent(string name) : base(name) {
        }

        protected CancellableEvent() : base() {}
    }
}

