/**
 * Entry Point for Snowbull.
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
using System.Net;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
            Dictionary<string, ZoneInitialiser> zones = new Dictionary<string, ZoneInitialiser>();
			zones.Add("w1", (server, oparent) => Login.LoginZoneActor.Props("w1", server, oparent));
            Snowbull instance = new Snowbull("Snowbull-Test", zones);
			instance.Bind(IPAddress.IPv6Any, 6112);
            Console.ReadLine();
        }
    }
}
