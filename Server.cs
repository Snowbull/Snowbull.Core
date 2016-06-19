/**
 * Observable Server for Snowbull.
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
using Akka.Actor;
using Akka.IO;

namespace Snowbull {
	internal sealed class Server : IServer, IContext {
		private IActorRef tcp;

		public string Name {
			get;
			private set;
		}

		public IActorRef ActorRef {
			get;
			private set;
		}

		public Server(string name, ActorSystem s) {
			tcp = s.Tcp();
			Name = name;
			ActorRef = s.ActorOf(ServerActor.Props(this), string.Format("server(Name={0})", Name));
		}

		public void Bind(IPEndPoint ep) {
			tcp.Tell(new Tcp.Bind(ActorRef, ep), ActorRef);
		}
	}
}

