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
    }
}

