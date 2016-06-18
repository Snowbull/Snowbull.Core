using System;

namespace Snowbull.API {
    public class IncorrectPasswordException : SnowbullException {
        public IConnection Connection {
            get;
            private set;
        }

        public IncorrectPasswordException(IConnection connection) : base() {
            Connection = connection;
        }

        public IncorrectPasswordException(IConnection connection, string message) : base(message) {
            Connection = connection;
        }

        public IncorrectPasswordException(IConnection connection, string message, Exception inner) : base(message, inner) {
            Connection = connection;
        }
    }
}

