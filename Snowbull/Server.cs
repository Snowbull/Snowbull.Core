using System;
using Akka;
using Akka.IO;
using Akka.Actor;
using System.Net;
using Akka.Event;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Xml;
using XmlMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<System.Xml.XmlDocument, Snowbull.API.Packets.Xml.XmlPacket>>;

namespace Snowbull {
    public class Server : ReceiveActor {
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        private readonly XmlMap xmlMap = API.Packets.PacketMapper.XmlMap();
        private readonly Dictionary<string, IActorRef> zones = new Dictionary<string, IActorRef>();

        public static Props Props() {
            return Akka.Actor.Props.Create(() => new Server());
        }

        public Server() {
            Receive<Tcp.Bound>(Bound);
            Receive<AddZone>(AddZone);
            Receive<Tcp.Connected>(Connected);
            Receive<Authenticate>(Authenticate);
            Receive<Disconnected>(Disconnected);
        }

        private void Bound(Tcp.Bound bound) {
            logger.Info("Server bound to " + bound.LocalAddress);
        }

        private void AddZone(AddZone zone) {
            zones.Add(zone.Name, Context.ActorOf(zone.Zone));
        }

        private void Connected(Tcp.Connected connected) {
            logger.Info("New client at " + connected.RemoteAddress + " connected!");
            IActorRef connection = Context.ActorOf(Connection.Props(Self, Sender, xmlMap));
            Sender.Tell(new Tcp.Register(connection));
        }

        private void Authenticate(Authenticate authenticate) {
            IActorRef zone = zones[authenticate.Request.Zone];
            if(zone != null)
                zone.Forward(authenticate);
        }

        private void Disconnected(Disconnected disconnected) {
            Context.Stop(disconnected.Actor);
        }

    }

    public class AddZone {
        public string Name {
            get;
            private set;
        }

        public Props Zone {
            get;
            private set;
        }

        public AddZone(string name, Props zone) {
            Name = name;
            Zone = zone;
        }
    }

    public class Disconnected {
        public IActorRef Actor {
            get;
            private set;
        }

        public Disconnected(IActorRef actor) {
            Actor = actor;
        }
    }
}

