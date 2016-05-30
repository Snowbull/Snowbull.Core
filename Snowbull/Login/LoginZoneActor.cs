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

namespace Snowbull.Login {
    sealed class LoginZoneActor : ZoneActor {

		public static Props Props(string name, IActorRef server, IActorRef oparent) {
			return Akka.Actor.Props.Create(() => new LoginZoneActor(name, server, oparent));
        }

		public LoginZoneActor(string name, IActorRef server, IActorRef oparent) : base(name, (n, a) => new LoginZone(n, a, oparent), server) {
        }

        protected override void Running() {
            base.Running();
            Receive<Authentication>(Authentication);
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

        private void Authentication(Authentication auth) {
            IActorRef connection = auth.Request.Sender;
            if(auth.Credentials != null) {
				logger.Debug("PASSWORD: " + auth.Credentials.Password);
                string hash = API.Cryptography.Hashing.HashPassword(auth.Credentials.Password, auth.Request.Key);
                if(auth.Request.Request.Password == hash) {
                    logger.Debug("Authenticated as '" + auth.Credentials.Username + "'!");
					connection.Tell(new Authenticated(LoginUserActor.Props(auth.Credentials.Id, auth.Credentials.Username, connection, Self, server, Observable.Actor), auth.Credentials), Self);
                }else{
                    logger.Info("Failed to identify as '" + auth.Request.Request.Username + "'.");
                    connection.Tell(new API.Packets.Xt.Send.Error(API.Errors.PASSWORD_WRONG, -1), Self);
                    connection.Tell(new Disconnect());
                }
            }else{
                logger.Info("Attempt to login as non existent user '" + auth.Request.Request.Username + "'.");
                connection.Tell(new API.Packets.Xt.Send.Error(API.Errors.NAME_NOT_FOUND, -1), Self);
                connection.Tell(new Disconnect());
            }
        }
    }
}

