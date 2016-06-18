/**
 * Server Bootstrap for Snowbull.
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
using System.Net;
using Akka.IO;
using System.Collections.Generic;

namespace Snowbull {
	delegate Zone ZoneInitialiser(IActorContext context, Server server);

    class Snowbull {
        private readonly ActorSystem actors = ActorSystem.Create("snowbull");
        private readonly Server server;

		public Server Server {
            get {
                return server;
            }
        }

        public Snowbull(string name, Dictionary<string, ZoneInitialiser> zones) {
			server = new Server(name, actors);
            foreach(KeyValuePair<string, ZoneInitialiser> zone in zones)
				server.ActorRef.Tell(new AddZone(zone.Key, zone.Value));
        }
    }
}

