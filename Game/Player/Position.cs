/**
 * Immutable position class for Snowbull.
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
    public sealed class Position {
        public int X {
            get;
            private set;
        }

        public int Y {
            get;
            private set;
        }

        public int Frame {
            get;
            private set;
        }

        public Position(int x, int y, int frame) {
            X = x;
            Y = y;
            Frame = frame;
        }

        public Position UpdateCoordinates(int x, int y) { // Usually these will change together.
            return new Position(x, y, Frame);
        }

        public Position UpdateFrame(int frame) {
            return new Position(X, Y, frame);
        }

        public override string ToString() {
            return string.Format("{0}|{1}|{2}", X, Y, Frame);
        }
    }
}

