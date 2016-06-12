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
		public static Props Props(int id, string username, Connection connection, Zone zone) {
			return Akka.Actor.Props.Create(() => new LoginUserActor(id, username, connection, zone));
        }

		public LoginUserActor(int id, string username, Connection connection, Zone zone) : base(id, username, (n, a) => new LoginUser(n, a, connection, (LoginZone) zone), connection, zone, (Server) zone.Server) {
        }

        protected override void PreStart() {
            base.PreStart();
			string key = API.Cryptography.Random.GenerateRandomKey(32);
			connection.InternalActor.Tell(new API.Packets.Xt.Send.Authentication.Login(id, key, ""));
        }
    }
}

