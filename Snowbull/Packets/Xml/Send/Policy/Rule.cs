/**
 * Base XML Policy Rule for Snowbull's Plugin API ("Snowbull.API").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull.API".
 * 
 * "Snowbull.API" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull.API" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull.API". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;

namespace Snowbull.API.Packets.Xml.Send.Policy {
	public abstract class Rule {
		internal string Tag {
			get;
			private set;
		}

		internal string Host {
			get;
			private set;
		}

		internal string Port {
			get;
			private set;
		}

		private Rule(string tag, string host, string port) {
			Tag = tag;
			Host = host;
			Port = port;
		}

		internal Rule(string tag, string host, int port) : this(tag, host, port.ToString()) {
		}

		internal Rule(string tag, string host) : this(tag, host, "*") {
		}

		internal Rule(string tag) : this(tag, "*", "*") {
		}
	}
}

