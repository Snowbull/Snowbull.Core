using System;

namespace Snowbull.API.Events {
	public abstract class Event : IEvent {
        public string Name {
            get;
            private set;
        }

        protected Event(string name) {
            Name = name;
        }

        protected Event() {
            Name = GetType().Name;
        }
    }
}

