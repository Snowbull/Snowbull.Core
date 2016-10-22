/**
 * Immutable Clothing class for Snowbull.
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

namespace Snowbull.Core.Game.Player {
    public sealed class Clothing {
        public int Colour {
            get;
            private set;
        }

        public int Head {
            get;
            private set;
        }

        public int Face {
            get;
            private set;
        }

        public int Neck {
            get;
            private set;
        }

        public int Body {
            get;
            private set;
        }

        public int Hands {
            get;
            private set;
        }

        public int Feet {
            get;
            private set;
        }

        public int Pin {
            get;
            private set;
        }

        public int Photo {
            get;
            private set;
        }

        public Clothing(int colour, int head, int face, int neck, int body, int hands, int feet, int pin, int photo) {
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

        public Clothing UpdateColour(int colour) {
            return new Clothing(colour, Head, Face, Neck, Body, Hands, Feet, Pin, Photo);
        }

        public Clothing UpdateHead(int head) {
            return new Clothing(Colour, head, Face, Neck, Body, Hands, Feet, Pin, Photo);
        }

        public Clothing UpdateFace(int face) {
            return new Clothing(Colour, Head, face, Neck, Body, Hands, Feet, Pin, Photo);
        }

        public Clothing UpdateNeck(int neck) {
            return new Clothing(Colour, Head, Face, neck, Body, Hands, Feet, Pin, Photo);
        }

        public Clothing UpdateBody(int body) {
            return new Clothing(Colour, Head, Face, Neck, body, Hands, Feet, Pin, Photo);
        }

        public Clothing UpdateHands(int hands) {
            return new Clothing(Colour, Head, Face, Neck, Body, hands, Feet, Pin, Photo);
        }

        public Clothing UpdateFeet(int feet) {
            return new Clothing(Colour, Head, Face, Neck, Body, Hands, feet, Pin, Photo);
        }

        public Clothing UpdatePin(int pin) {
            return new Clothing(Colour, Head, Face, Neck, Body, Hands, Feet, pin, Photo);
        }

        public Clothing UpdatePhoto(int photo) {
            return new Clothing(Colour, Head, Face, Neck, Body, Hands, Feet, Pin, photo);
        }

        public override string ToString() {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", Colour, Head, Face, Neck, Body, Hands, Feet, Pin, Photo);
        }
    }
}

