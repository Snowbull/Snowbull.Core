using System;

namespace Snowbull.Data.Models.Immutable {
    public class ImmutableCredentials {
        public int Id {
            get;
            private set;
        }

        public string Username {
            get;
            private set;
        }

        public string Password {
            get;
            private set;
        }

        public ImmutableCredentials(int id, string username, string password) {
            Id = id;
            Username = username;
            Password = password;
        }
    }
}

