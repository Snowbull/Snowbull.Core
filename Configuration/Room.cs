using System.Configuration;

namespace Snowbull.Core.Configuration {
    public class Room : ConfigurationElement {
        [ConfigurationProperty("id", IsRequired=true)]
        public int Id {
            get { return (int) this["id"]; }
        }

        [ConfigurationProperty("external", IsRequired=true)]
        public int ExternalId {
            get { return (int) this["external"]; }
        }

        [ConfigurationProperty("name", IsRequired=true)]
        public string Name {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("capacity", IsRequired=true)]
        public int Capacity {
            get { return (int) this["capacity"]; }
        }

        [ConfigurationProperty("member", IsRequired=true)]
        public bool Member {
            get { return (bool) this["member"]; }
        }
    }
}

