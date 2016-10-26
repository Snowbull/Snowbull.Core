using System.Configuration;

namespace Snowbull.Core.Configuration {
    public class ClubPenguinConfigurationSection : ConfigurationSection {
        public static ClubPenguinConfigurationSection GetConfiguration() {
            return (ClubPenguinConfigurationSection) ConfigurationManager.GetSection("clubPenguin") ?? new ClubPenguinConfigurationSection();
        }

        [ConfigurationProperty("rooms")]
        [ConfigurationCollection(typeof(Rooms), AddItemName = "room")]
        public Rooms Rooms {
            get {
                return this["rooms"] as Rooms;
            }
        }

        [ConfigurationProperty("items")]
        [ConfigurationCollection(typeof(Items), AddItemName = "item")]
        public Items Items {
            get {
                return this["items"] as Items;
            }
        }
    }
}

