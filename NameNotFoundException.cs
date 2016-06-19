using System;

namespace Snowbull.Core {
    public class NameNotFoundException : SnowbullException {
        public IConnection Connection {
            get;
            private set;
        }

        public string Username {
            get;
            private set;
        }

        public NameNotFoundException(IConnection connection, string username) : base() {
            Connection = connection;
            Username = username;
        }

        public NameNotFoundException(IConnection connection, string username, string message) : base(message) {
            Connection = connection;
            Username = username;
        }

        public NameNotFoundException(IConnection connection, string username, string message, Exception inner) : base(message, inner) {
            Connection = connection;
            Username = username;
        }
    }
}

