﻿/**
 * Observable Connection for Snowbull.
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
using XmlMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<System.Xml.XmlDocument, Snowbull.API.Packets.Xml.XmlPacket>>;
using XtMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<Snowbull.API.Packets.Xt.XtData, Snowbull.API.Packets.Xt.XtPacket>>;

namespace Snowbull {
	internal class Connection : API.IConnection, IContext {
		public IActorRef ActorRef {
			get;
			private set;
		}

		public API.IServer Server {
			get;
			private set;
		}

		public EndPoint Address {
			get;
			private set;
		}

		public Connection(IActorContext c, IActorRef socket, EndPoint address, Server server, XmlMap xmlMap, XtMap xtMap) {
			Server = server;
			Address = address;
			ActorRef = c.ActorOf(ConnectionActor.Props(this, socket, xmlMap, xtMap));
		}

		internal void Send(API.Packets.ISendPacket packet, IActorRef sender) {
			ActorRef.Tell(packet, sender);
		}

		public void Send(API.Packets.ISendPacket packet) {
			ActorRef.Tell(packet);
		}

		public void Close() {
			ActorRef.Tell(new Disconnect());
		}
	}
}

