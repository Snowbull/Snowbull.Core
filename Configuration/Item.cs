using System.Configuration;

namespace Snowbull.Core {
    public class Item : ConfigurationElement {
        [ConfigurationProperty("id", IsRequired=true, IsKey=true)]
        public int Id {
            get { return (int) this["id"]; }
        }

        [ConfigurationProperty("description", IsRequired=true)]
        public string Description {
            get { return (string) this["description"]; }
        }

        [ConfigurationProperty("type", IsRequired=true)]
        public string Type {
            get { return (string) this["type"]; }
        }

        [ConfigurationProperty("price", IsRequired=true)]
        public int Price {
            get { return (int) this["price"]; }
        }

        [ConfigurationProperty("member", IsRequired=true)]
        public bool Member {
            get { return (bool) this["member"]; }
        }
    }
}

