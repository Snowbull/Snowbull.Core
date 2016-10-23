/**
 * Immutable game user context for Snowbull.
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

using Akka.Actor;

namespace Snowbull.Core.Game {
    /// <summary>
    /// Immutable game user context.
    /// </summary>
	public class GameUser : User {
        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.GameUser"/> class.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="username">Username.</param>
        /// <param name="c">C.</param>
        /// <param name="connection">Connection.</param>
        /// <param name="zone">Zone.</param>
		public GameUser(int id, string username, IActorContext c, Connection connection, Zone zone) : base(id, username, connection, zone) {
			ActorRef = c.ActorOf(GameUserActor.Props(this), string.Format("user(Id={0},Username={1}", id, username));
			connection.ActorRef.Tell(new Authenticated(this), ActorRef);
			connection.ActorRef.Tell(new Packets.Xt.Send.Authentication.Login(), ActorRef);
		}
	}
}

