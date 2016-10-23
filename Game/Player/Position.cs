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
    /// <summary>
    /// Immutable player Position.
    /// </summary>
    public sealed class Position {
        /// <summary>
        /// Gets the X coordinate.
        /// </summary>
        /// <value>The X coordinate.</value>
        public int X {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Y coordinate.
        /// </summary>
        /// <value>The Y coordinate.</value>
        public int Y {
            get;
            private set;
        }

        /// <summary>
        /// Gets the player's current frame number.
        /// </summary>
        /// <value>The player's current frame number.</value>
        public int Frame {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Player.Position"/> class.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="frame">The player's current frame.</param>
        public Position(int x, int y, int frame) {
            X = x;
            Y = y;
            Frame = frame;
        }

        /// <summary>
        /// Updates the coordinates.
        /// </summary>
        /// <returns>A new Position instance with the updated coordinates.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Position UpdateCoordinates(int x, int y) { // Usually these will change together.
            return new Position(x, y, Frame);
        }

        /// <summary>
        /// Updates the frame.
        /// </summary>
        /// <returns>A new Position instance with the updated frame.</returns>
        /// <param name="frame">The new frame.</param>
        public Position UpdateFrame(int frame) {
            return new Position(X, Y, frame);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Snowbull.Core.Game.Player.Position"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Snowbull.Core.Game.Player.Position"/>.</returns>
        public override string ToString() {
            return string.Format("{0}|{1}|{2}", X, Y, Frame);
        }
    }
}

