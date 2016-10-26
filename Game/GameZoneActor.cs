/**
 * Game zone actor for Snowbull.
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
using System.Data.Entity;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull.Core.Game {
    sealed class GameZoneActor : ZoneActor {
        private readonly Dictionary<int, Rooms.Room> rooms = new Dictionary<int, Rooms.Room>();

		public static Props Props(GameZone zone) {
			return Akka.Actor.Props.Create(() => new GameZoneActor(zone));
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Core.Game.GameZoneActor"/> class.
        /// </summary>
        /// <param name="zone">Immutable zone context.</param>
		public GameZoneActor(GameZone zone) : base(zone) {
            Configuration.ClubPenguinConfigurationSection config = Configuration.ClubPenguinConfigurationSection.GetConfiguration();
            foreach(Configuration.Room setting in config.Rooms) {
                Rooms.Room room = new Rooms.Room(setting.Id, setting.ExternalId, setting.Name, zone, setting.Capacity, Context);
                rooms.Add(room.ExternalId, room);
            }
        }

        /// <summary>
        /// Regular running state/behaviour.
        /// </summary>
        protected override void Running() {
            base.Running();
            Receive<Rooms.JoinRoom>(new Action<Rooms.JoinRoom>(JoinRoom));
        }

		protected override void Authenticate(Authenticate auth) {
			// Temporary, the login key will be fetched from the login server. TODO
			Data.SnowbullContext db = new Data.SnowbullContext();
			db.Users.FirstAsync<Data.Models.User>(u => u.Username == auth.Request.Username).ContinueWith<Authentication>(
				t => {
					Data.Models.Immutable.ImmutableCredentials credentials = t.IsFaulted ? null : new Data.Models.Immutable.ImmutableCredentials(t.Result);
                    db.Dispose();
					return new Authentication(credentials, auth);
				}
			).PipeTo(Self);
        }

		protected override User Authentication(Authenticate request, Data.Models.Immutable.ImmutableCredentials credentials) {
			// TEMPORARY: GAME SERVER DOES *NOT* CURRENTLY CHECK CREDENTIALS: TODO
			if(credentials != null) {
				// if(login key is correct) {
					logger.Info("Sucessfully authenticated as '{0}'!", credentials.Username);
					return new GameUser(credentials.Id, credentials.Username, Context, request.Sender, zone);
				// }else{
				//     throw new API.IncorrectPasswordException ...
				// }
			} else {
				throw new NameNotFoundException(request.Sender, request.Request.Username, string.Format("User '{0}' was not found!", request.Request.Username));
			}
		}

        private void JoinRoom(Rooms.JoinRoom jr) {
            Rooms.Room room = rooms[jr.ExternalId];
            if(room != null)
                room.ActorRef.Forward(jr);
        }
    }
}

