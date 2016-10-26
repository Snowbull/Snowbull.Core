using System.Configuration;

namespace Snowbull.Core {
    public class Items : ConfigurationElementCollection {
        public Item this[int i] {
            get {
                return (Item) BaseGet(i);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new Item();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return (element as Item).Id;
        }
    }
}

