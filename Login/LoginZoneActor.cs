/**
 * Login Zone Akka Actor for Snowbull.
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
using System.Data.Entity;

namespace Snowbull.Core.Login {
    sealed class LoginZoneActor : ZoneActor {
		public static Props Props(LoginZone zone) {
			return Akka.Actor.Props.Create(() => new LoginZoneActor(zone));
        }

		public LoginZoneActor(LoginZone zone) : base(zone) {
        }

        protected override void Running() {
            base.Running();
        }

        protected override void Authenticate(Authenticate auth) {
            Data.SnowbullContext db = new Data.SnowbullContext();
            db.Users.FirstAsync<Data.Models.User>(u => u.Username == auth.Request.Username).ContinueWith<Authentication>(
                t => {
                    Data.Models.Immutable.ImmutableCredentials credentials = t.IsFaulted ? null : new Data.Models.Immutable.ImmutableCredentials(t.Result);
                    return new Authentication(credentials, auth);
                }
            ).PipeTo(Self);
        }

		protected override User Authentication(Authenticate request, Data.Models.Immutable.ImmutableCredentials credentials) {
            if(credentials != null) {
				string hash = Cryptography.Hashing.HashPassword(credentials.Password, request.Key);
                if(request.Request.Password == hash) {
                    logger.Debug("Authenticated as '" + credentials.Username + "'!");
					return new LoginUser(credentials.Id, credentials.Username, Context, request.Sender, (LoginZone) zone);
                }else{
					throw new IncorrectPasswordException(request.Sender, string.Format("Peer at {0} failed to identify as '{1}'.", request.Sender.Address, credentials.Username));
                }
            }else{
				throw new NameNotFoundException(request.Sender, request.Request.Username, string.Format("Attempt to login as non existent user '{0}'.", request.Request.Username));
            }
        }
    }
}

