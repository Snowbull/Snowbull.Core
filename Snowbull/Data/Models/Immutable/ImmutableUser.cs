using System;

namespace Snowbull.Data.Models.Immutable {
    public class ImmutableUser {
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

        public DateTime Creation {
            get;
            private set;
        }

        public TimeSpan Played {
            get;
            private set;
        }

        public ImmutableUser(int id, string username, string password, DateTime creation, TimeSpan played) {
            Id = id;
            Username = username;
            Password = password;
            Creation = creation;
            Played = played;
        }

        public ImmutableUser(User user) : this(user.Id, user.Username, user.Password, user.Creation, user.Played) {
            
        }
    }
}

