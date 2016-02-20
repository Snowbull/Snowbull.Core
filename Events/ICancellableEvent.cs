using System;

namespace Snowbull.API.Events {
    public interface ICancellableEvent : IEvent {
        bool Cancelled {
            get;
        }
    }
}

