/**
 * Immutable player class for Snowbull.
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
    /// Immutable player class, designed to be passed between UserActors and RoomActors to keep track of state.
    /// </summary>
    public sealed class Player {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <value>The user.</value>
        public GameUser User {
            get;
            private set;
        }

        /// <summary>
        /// Gets the player's clothing.
        /// </summary>
        /// <value>The player's clothing.</value>
        public Clothing.Costume Costume {
            get;
            private set;
        }

        /// <summary>
        /// Gets the player's position.
        /// </summary>
        /// <value>The player's position.</value>
        public Position Position {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.Player.Player"/> class.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="clothing">Player's clothing.</param>
        /// <param name="position">Player's position.</param>
        public Player(GameUser user, Clothing.Costume costume, Position position) {
            User = user;
            Costume = costume;
            Position = position;
        }

        /// <summary>
        /// Updates the player's costume.
        /// </summary>
        /// <returns>A new Player instance containing the updated Costume object.</returns>
        /// <param name="costume">Costume.</param>
        public Player UpdateCostume(Clothing.Costume costume) {
            return new Player(User, costume, Position);
        }

        /// <summary>
        /// Updates the player's position.
        /// </summary>
        /// <returns>A new Player instance containing the updated Position object.</returns>
        /// <param name="position">Position.</param>
        public Player UpdatePosition(Position position) {
            return new Player(User, Costume, position);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Snowbull.Core.Game.Player.Player"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Snowbull.Core.Game.Player.Player"/>.</returns>
        public override string ToString() {
            int days = 0; // Temporary, will add ranks/days.
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", User.Id, User.Username, 1, Costume, Position, 1, days, 0, 0);
        }
    }
}

