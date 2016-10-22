using System.Configuration;

namespace Snowbull.Core.Configuration {
    public class Room : ConfigurationElement {
        [ConfigurationProperty("id", IsRequired=true)]
        public string Id {
            get { return this["id"] as string; }
        }

        [ConfigurationProperty("external", IsRequired=true)]
        public string ExternalId {
            get { return this["external"] as string; }
        }

        [ConfigurationProperty("name", IsRequired=true)]
        public string Name {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("capacity", IsRequired=true)]
        public string Capacity {
            get { return this["capacity"] as string; }
        }

        [ConfigurationProperty("member", IsRequired=true)]
        public string Member {
            get { return this["member"] as string; }
        }
    }
}

