using System.Configuration;

namespace Snowbull.Core.Configuration {
    public class Rooms : ConfigurationElementCollection {
        public Room this[int i] {
            get {
                return BaseGet(i) as Room;
            }
            set {
                if(BaseGet(i) != null)
                    BaseRemoveAt(i);
                BaseAdd(i, value);
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new Room();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return (element as Room).Id;
        }
    }
}

