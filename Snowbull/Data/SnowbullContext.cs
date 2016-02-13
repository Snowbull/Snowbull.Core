using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Snowbull.Data {
    public class SnowbullContext : DbContext {
        static SnowbullContext() {
            Database.SetInitializer<SnowbullContext>(null);
        }

        public SnowbullContext() : base("name=Snowbull") {
        }

        public DbSet<Models.User> Users {
            get;
            set;
        }
    }
}

