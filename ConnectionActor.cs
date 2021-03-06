﻿/**
 * Connection Akka Actor for Snowbull.
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
using Akka.IO;
using System.Text;
using Akka.Event;
using XmlMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<System.Xml.XmlDocument, Snowbull.Core.Packets.Xml.XmlPacket>>;
using XtMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<Snowbull.Core.Packets.Xt.XtData, Snowbull.Core.Packets.Xt.XtPacket>>;
using System.Xml;
using Snowbull.Core.Packets.Xml.Receive.Authentication;
using Snowbull.Core.Packets;

namespace Snowbull.Core {
   	public sealed class ConnectionActor : SnowbullActor {
		private readonly Connection connection;
        private readonly IActorRef socket;
        private User user;
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        private readonly XmlMap xmlMap;
        private readonly XtMap xtMap;
        private string buffer = "";
        private int authenticationPackets = 0;
        private readonly string key = Cryptography.Random.GenerateRandomKey(10);

		public static Props Props(Connection connection, IActorRef socket, XmlMap xmlMap, XtMap xtMap) {
			return Akka.Actor.Props.Create(() => new ConnectionActor(connection, socket, xmlMap, xtMap));
        }

		public ConnectionActor(Connection connection, IActorRef socket, XmlMap xmlMap, XtMap xtMap) : base() {
			this.connection = connection;
            this.socket = socket;
            this.xmlMap = xmlMap;
            this.xtMap = xtMap;
            BecomeStacked(APICheck);
        }

        private void Running() {
            Receive<Tcp.Received>(new Action<Tcp.Received>(Received));
            Receive<Tcp.PeerClosed>(new Action<Tcp.PeerClosed>(Closed));
            Receive<Packets.ISendPacket>(new Action<ISendPacket>(Send));
            Receive<RawPacketReceived>(user == null ? new Action<RawPacketReceived>(ProcessUnauthenticatedPacket) : new Action<RawPacketReceived>(ProcessAuthenticatedPacket));
			Receive<Terminated>(new Action<Terminated>(Terminated));
        }

        private void Received(Tcp.Received received) {
            string recv = Encoding.UTF8.GetString(received.Data.ToArray());
            string[] data = (buffer + recv).Split('\0');
            for(int i = 0; i < data.Length - 1; i++) {
                string packet = data[i];
                #if DEBUG
                logger.Debug("Received: " + packet);
                #endif
                Self.Tell(new RawPacketReceived(packet), Self);
            }
            buffer = data[data.Length - 1];
        }

        private void ProcessUnauthenticatedPacket(RawPacketReceived p) {
            string packet = p.Data;
            if(authenticationPackets < 6) {
                if(packet.StartsWith("<")) // Better than throwing it at a parser to determine it.
                    ProcessXml(packet);
            }else{
				logger.Info("Client at '" + connection.Address + "' disconnected for sending too many authentication packets.");
                socket.Tell(Tcp.Close.Instance);
            }
            authenticationPackets++;
        }

        private void ProcessAuthenticatedPacket(RawPacketReceived p) {
            string packet = p.Data;
            if(packet.StartsWith("%")) 
                ProcessXt(packet);
        }

        /// <summary>
        /// Processes an xml packet.
        /// </summary>
        /// <param name="xml">Xml.</param>
        private void ProcessXml(string xml) {
            XmlReaderSettings settings = new XmlReaderSettings();
            // Defense against XML bombs.
            settings.DtdProcessing = DtdProcessing.Prohibit;
            settings.XmlResolver = null;
            using(System.IO.StringReader sreader = new System.IO.StringReader(xml)) {
                using(XmlReader xreader = XmlReader.Create(sreader, settings)) {
                    XmlDocument document = new XmlDocument();
                    document.Load(xreader);
                    XmlElement element = document.DocumentElement;
					switch(element.Name) {
						case "policy-file-request":
							Packets.Xml.Send.Policy.XmlPolicyFile policy = Packets.Xml.Send.Policy.XmlPolicyFile.Create(new Packets.Xml.Send.Policy.Allow[] { new Packets.Xml.Send.Policy.Allow() });
							Self.Tell(policy, Self);
						break;
						case "msg":
		                    // We need to get the body node to find the action.
							XmlNode body = Packets.Xml.XmlMessage.Verify(element);
							string action = body.Attributes["action"].Value;
							Packets.Xml.XmlPacket packet = xmlMap[action](document);
							Self.Tell(packet, Self);
						break;
					}
                }
            }
        }

        private void ProcessXt(string xt) {
			if(user != null) {
				Packets.Xt.XtData parser = Packets.Xt.XtData.Parse(xt, Packets.Xt.From.Client);
				Func<Packets.Xt.XtData, Packets.Xt.XtPacket> processor;
				if(xtMap.TryGetValue(parser.Command, out processor)) {
					Packets.Xt.XtPacket packet = processor(parser);
					user.ActorRef.Tell(packet, Self);
				} else {
					Logger.Warning("Unhandled extension packet: {0}.", xt);
				}
			}
        }

        /// <summary>
        /// Send the specified packet.
        /// </summary>
        /// <param name="packet">Packet.</param>
        private void Send(Packets.ISendPacket packet) {
            #if DEBUG
            logger.Debug("Sending: " + packet);
            #endif
            byte[] bytes = Encoding.UTF8.GetBytes(packet + "\0");
            ByteStringBuilder bsb = new ByteStringBuilder();
            bsb.Append(bytes);
            socket.Tell(Tcp.Write.Create(bsb.Result()));
        }

        /// <summary>
        /// Sets the actor to expect an API version check.
        /// </summary>
        private void APICheck() {
            Receive<VersionCheck>(new Action<VersionCheck>(VersionCheck));
            Running();
        }


        /// <summary>
        /// Checks the client's API version.
        /// </summary>
        /// <param name="verChk">Version check packet.</param>
        private void VersionCheck(VersionCheck verChk) {
            // Tell the client that the version is fine.
            Self.Tell(Packets.Xml.Send.Authentication.ApiOK.Create());
            // Expect random key request.
            UnbecomeStacked();
            BecomeStacked(KeyAgreement);
        }

        /// <summary>
        /// Sets the actor to expect a random key request.
        /// </summary>
        private void KeyAgreement() {
            Receive<RandomKey>(new Action<RandomKey>(RandomKey));
            Running();
        }

        /// <summary>
        /// Handles a request for a random key.
        /// </summary>
        /// <param name="rndk">Random key request packet.</param>
        private void RandomKey(RandomKey rndk) {
            // Tell the client the random key.
            Self.Tell(Packets.Xml.Send.Authentication.RandomKey.Create(key));
            // Expect a login request.
            UnbecomeStacked();
            BecomeStacked(Authentication);
        }

        /// <summary>
        /// Sets the actor to expect an authentication packet.
        /// </summary>
        private void Authentication() {
            Receive<Packets.Xml.Receive.Authentication.Login>(new Action<Packets.Xml.Receive.Authentication.Login>(Login));
            Running();
        }

        private void Login(Packets.Xml.Receive.Authentication.Login login) {
            UnbecomeStacked();
            BecomeStacked(Authenticating);
			((Server) connection.Server).ActorRef.Tell(new Authenticate(login, connection, key), Self);
        }

        private void Authenticating() {
            Receive<Authenticated>(new Action<Authenticated>(Authenticate));
            Running();
        }

        private void Authenticate(Authenticated auth) {
			user = auth.User;
			Context.Watch(user.ActorRef);
            UnbecomeStacked();
            BecomeStacked(Authenticated);
        }

        private void Authenticated() {
            Running();
        }

		private void Terminated(Terminated t) {
            if(user != null)
                if(user.ActorRef == t.ActorRef)
                    socket.Tell(Tcp.Close.Instance);
		}

        private void Closed(Tcp.PeerClosed closed) {
            logger.Info("Peer at '" + connection.Address + "' closed the connection.");
            UnbecomeStacked();
            Context.Stop(Self);
        }
    }

    public class RawPacketReceived {
        public string Data {
            get;
            private set;
        }

        public RawPacketReceived(string data) {
            Data = data;
        }
    }

    class Authenticated {
        public User User {
            get;
            private set;
        }

		public Authenticated(User user) {
            User = user;
        }
    }
}

