/**
 * Immutable Costume class for Snowbull.
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of Snowbull.
 * 
 * Snowbull is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * Snowbull is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Snowbull. If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System.Collections.Immutable;

namespace Snowbull.Core.Game.Player.Clothing {
    public sealed class Costume {
        public Item Colour {
            get;
            private set;
        }

        public Item Head {
            get;
            private set;
        }

        public Item Face {
            get;
            private set;
        }

        public Item Neck {
            get;
            private set;
        }

        public Item Body {
            get;
            private set;
        }

        public Item Hands {
            get;
            private set;
        }

        public Item Feet {
            get;
            private set;
        }

        public Item Pin {
            get;
            private set;
        }

        public Item Photo {
            get;
            private set;
        }

        public Costume(Item colour, Item head, Item face, Item neck, Item body, Item hands, Item feet, Item pin, Item photo) {
            Colour = colour;
            Head = head;
            Face = face;
            Neck = neck;
            Body = body;
            Hands = hands;
            Feet = feet;
            Pin = pin;
            Photo = photo;
        }

        public Costume(Data.Models.Clothing clothing, ImmutableDictionary<int, Item> items) : this(
            items[clothing.Colour],
            items[clothing.Head],
            items[clothing.Face],
            items[clothing.Neck],
            items[clothing.Body],
            items[clothing.Hands],
            items[clothing.Feet],
            items[clothing.Pin],
            items[clothing.Photo]
        ) {}

        public Costume UpdateColour(Item colour) {
            return new Costume(colour, Head, Face, Neck, Body, Hands, Feet, Pin, Photo);
        }

        public Costume UpdateHead(Item head) {
            return new Costume(Colour, head, Face, Neck, Body, Hands, Feet, Pin, Photo);
        }

        public Costume UpdateFace(Item face) {
            return new Costume(Colour, Head, face, Neck, Body, Hands, Feet, Pin, Photo);
        }

        public Costume UpdateNeck(Item neck) {
            return new Costume(Colour, Head, Face, neck, Body, Hands, Feet, Pin, Photo);
        }

        public Costume UpdateBody(Item body) {
            return new Costume(Colour, Head, Face, Neck, body, Hands, Feet, Pin, Photo);
        }

        public Costume UpdateHands(Item hands) {
            return new Costume(Colour, Head, Face, Neck, Body, hands, Feet, Pin, Photo);
        }

        public Costume UpdateFeet(Item feet) {
            return new Costume(Colour, Head, Face, Neck, Body, Hands, feet, Pin, Photo);
        }

        public Costume UpdatePin(Item pin) {
            return new Costume(Colour, Head, Face, Neck, Body, Hands, Feet, pin, Photo);
        }

        public Costume UpdatePhoto(Item photo) {
            return new Costume(Colour, Head, Face, Neck, Body, Hands, Feet, Pin, photo);
        }

        public override string ToString() {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", Colour.Id, Head.Id, Face.Id, Neck.Id, Body.Id, Hands.Id, Feet.Id, Pin.Id, Photo.Id);
        }
    }
}

