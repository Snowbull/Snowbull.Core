﻿/**
 * Observable Zone for Snowbull.
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

namespace Snowbull.Core {
	public abstract class Zone : IZone, IContext {
		public string Name {
			get;
			private set;
		}

		public IActorRef ActorRef {
			get;
			protected set;
		}

		public IServer Server {
			get;
			private set;
		}

		public Zone(string name, Server server) {
			Name = name;
			Server = server;
		}
	}
}

