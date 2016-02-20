using System;

namespace Snowbull.API.Events.Authentication {
    public class Authenticated : Event {
        public string Username {
            get;
            private set;
        }

        public Authenticated(string username) {
            Username = username;
        }
    }
}

