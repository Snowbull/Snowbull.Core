/**
 * Server Akka Actor for Snowbull.
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
using Akka;
using Akka.IO;
using Akka.Actor;
using System.Net;
using Akka.Event;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Xml;
using XmlMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<System.Xml.XmlDocument, Snowbull.Core.Packets.Xml.XmlPacket>>;
using XtMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<Snowbull.Core.Packets.Xt.XtData, Snowbull.Core.Packets.Xt.XtPacket>>;

namespace Snowbull.Core {
	sealed class ServerActor : SnowbullActor {
		private readonly Server server;
        private readonly XmlMap xmlMap = Packets.PacketMapper.XmlMap();
        private readonly XtMap xtMap = Packets.PacketMapper.XtMap();
        private readonly Dictionary<string, Zone> zones = new Dictionary<string, Zone>();

		public static Props Props(Server server) {
            return Akka.Actor.Props.Create(() => new ServerActor(server));
        }

		public ServerActor(Server server) : base() {
			this.server = server;
            Receive<Tcp.Bound>(new Action<Tcp.Bound>(Bound));
            Receive<AddZone>(new Action<AddZone>(AddZone));
            Receive<Tcp.Connected>(new Action<Tcp.Connected>(Connected));
            Receive<Authenticate>(new Action<Authenticate>(Authenticate));
            Receive<Disconnected>(new Action<Disconnected>(Disconnected));
        }

		protected override SupervisorStrategy SupervisorStrategy() {
			return new OneForOneStrategy(
				Decider.From(ex => {
					if(ex is IncorrectPasswordException) {
						IConnection connection = ((IncorrectPasswordException) ex).Connection;
						connection.Send(new Packets.Xt.Send.Error(Errors.PASSWORD_WRONG, -1));
						connection.Close();
						return Directive.Resume;
					}else if(ex is NameNotFoundException) {
						IConnection connection = ((NameNotFoundException) ex).Connection;
						connection.Send(new Packets.Xt.Send.Error(Errors.NAME_NOT_FOUND, -1));
						connection.Close();
						return Directive.Resume;
					}else{
						return Directive.Restart;
					}
				})
			);
		}

        private void Bound(Tcp.Bound bound) {
            Logger.Info("Server bound to " + bound.LocalAddress);
        }

        private void AddZone(AddZone az) {
			Zone zone = az.Zone(Context, server);
			zones.Add(zone.Name, zone);
        }

        private void Connected(Tcp.Connected connected) {
            Logger.Info("New client at " + connected.RemoteAddress + " connected!");
			Connection connection = new Connection(Context, Sender, connected.RemoteAddress, server, xmlMap, xtMap);
			Sender.Tell(new Tcp.Register(connection.ActorRef));
        }

        private void Authenticate(Authenticate authenticate) {
            Zone zone = zones[authenticate.Request.Zone];
            if(zone != null)
                zone.ActorRef.Forward(authenticate);
        }

        private void Disconnected(Disconnected disconnected) {
            Context.Stop(disconnected.Actor);
        }

    }

    internal class AddZone {
        public string Name {
            get;
            private set;
        }

        public ZoneInitialiser Zone {
            get;
            private set;
        }

        public AddZone(string name, ZoneInitialiser zone) {
            Name = name;
            Zone = zone;
        }
    }

    internal class Disconnected {
        public IActorRef Actor {
            get;
            private set;
        }

        public Disconnected(IActorRef actor) {
            Actor = actor;
        }
    }
}

