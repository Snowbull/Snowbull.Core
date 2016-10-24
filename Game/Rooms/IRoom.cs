/**
 * Immutable Room context interface for Snowbull.
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

namespace Snowbull.Core.Game.Rooms {
    /// <summary>
    /// Immutable Room context interface.
    /// </summary>
    public interface IRoom : IContext {
        /// <summary>
        /// Gets the Room's internal identifier.
        /// </summary>
        /// <value>The Room's internal identifier.</value>
        int InternalId {
            get;
        }

        /// <summary>
        /// Gets the Room's external identifier.
        /// </summary>
        /// <value>The Room's external identifier.</value>
        int ExternalId {
            get;
        }

        /// <summary>
        /// Gets the Room's name.
        /// </summary>
        /// <value>The Room's name.</value>
        string Name {
            get;
        }

        /// <summary>
        /// Gets the zone that the Room is in.
        /// </summary>
        /// <value>The zone that the Room is in.</value>
        IZone Zone {
            get;
        }
    }
}

