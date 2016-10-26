namespace Snowbull.Core.Game.Player.Clothing {
    public class Item {
        public int Id {
            get;
            private set;
        }

        public string Description {
            get;
            private set;
        }

        public int Price {
            get;
            private set;
        }

        public Type Type {
            get;
            private set;
        }

        public bool Member {
            get;
            private set;
        }

        public Item(int id, string description, int price, Type type, bool member) {
            Id = id;
            Description = description;
            Price = price;
            Type = type;
            Member = member;
        }
    }

    public enum Type {
        Colour,
        Head,
        Face,
        Neck,
        Body,
        Hands,
        Feet,
        Photo,
        Pin,
        Other
    }
}

