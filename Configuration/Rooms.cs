using System.Configuration;

namespace Snowbull.Core.Configuration {
    public class Rooms : ConfigurationElementCollection {
        public Room this[int i] {
            get {
                return (Room) BaseGet(i);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new Room();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((Room) element).Id;
        }
    }
}

