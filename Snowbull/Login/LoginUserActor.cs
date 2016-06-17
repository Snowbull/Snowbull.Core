/**
 * Login User Akka Actor for Snowbull.
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

using System;
using Akka.Actor;

namespace Snowbull.Login {
    sealed class LoginUserActor : UserActor {
        /// <summary>
        /// Creates a user actor with id, username, connection, zone and server props.
        /// </summary>
        /// <param name="connection">The user's connection.</param>
        /// <param name="server">The server the user is connected to.</param>
        /// <param name="zone">The zone the user is in.</param> 
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username.</param>  
		public static Props Props(LoginUser user) {
			return Akka.Actor.Props.Create(() => new LoginUserActor(user));
        }

		public LoginUserActor(LoginUser user) : base(user) {
        }
    }
}

