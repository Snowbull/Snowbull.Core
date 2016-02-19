using System;

namespace Snowbull.Data.Models.Immutable {
    public class ImmutableUser : ImmutableCredentials {
        public DateTime Creation {
            get;
            private set;
        }

        public TimeSpan Played {
            get;
            private set;
        }

        public ImmutableUser(int id, string username, string password, DateTime creation, TimeSpan played) : base(id, username, password) {
            Creation = creation;
            Played = played;
        }

        public ImmutableUser(User user) : this(user.Id, user.Username, user.Password, user.Creation, user.Played) {
            
        }
    }
}

