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
    public sealed class Player {
        public GameUser User {
            get;
            private set;
        }

        public Clothing Clothing {
            get;
            private set;
        }

        public Position Position {
            get;
            private set;
        }

        public Player(GameUser user, Clothing clothing, Position position) {
            User = user;
            Clothing = clothing;
            Position = position;
        }

        public Player UpdateClothing(Clothing clothing) {
            return new Player(User, clothing, Position);
        }

        public Player UpdatePosition(Position position) {
            return new Player(User, Clothing, position);
        }

        public override string ToString() {
            int days = 0; // Temporary, will add ranks/days.
            return string.Format("{0}|{1}|{2}|{3}|{4}|{7}|{8}|{9}|{10}", User.Id, User.Username, 1, Clothing, Position, 1, days, 0, 0);
        }
    }
}

