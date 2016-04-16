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
using XtMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<Snowbull.API.Packets.Xt.XtData, Snowbull.API.Packets.Xt.XtPacket>>;

namespace Snowbull {
	sealed class ServerActor : SnowbullActor {
        private readonly XmlMap xmlMap = API.Packets.PacketMapper.XmlMap();
        private readonly XtMap xtMap = API.Packets.PacketMapper.XtMap();
        private readonly Dictionary<string, IActorRef> zones = new Dictionary<string, IActorRef>();

		public static Props Props(string name) {
            return Akka.Actor.Props.Create(() => new ServerActor(name));
        }

		public ServerActor(string name) : base(new Server(name, Context)) {
            Receive<Tcp.Bound>(Bound);
            Receive<AddZone>(AddZone);
            Receive<Tcp.Connected>(Connected);
            Receive<Authenticate>(Authenticate);
            Receive<Disconnected>(Disconnected);
			Type[] types = API.Assemblies.Load("Plugins/").Get();
			IActorRef[] actors = new IActorRef[types.Length];
			for(int i = 0; i < types.Length; i++) {
				Type t = types[i];
				actors[i] = Context.ActorOf(API.Observer.ObserverActor.Props((API.IServer)Observable, c => (API.Observer.Observer)Activator.CreateInstance(t, new object[] { c })), "plugin(" + i + ")");
			}
        }

        private void Bound(Tcp.Bound bound) {
            Logger.Info("Server bound to " + bound.LocalAddress);
        }

        private void AddZone(AddZone zone) {
			zones.Add(zone.Name, Context.ActorOf(zone.Zone(Self, Observable.Actor), "zone(" + zone.Name + ")"));
        }

        private void Connected(Tcp.Connected connected) {
            Logger.Info("New client at " + connected.RemoteAddress + " connected!");
			IActorRef connection = Context.ActorOf(ConnectionActor.Props(Self, Sender, connected.RemoteAddress, xmlMap, xtMap, Observable.Actor), "connection(" + connected.RemoteAddress + ")");
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

        public ZoneInitialiser Zone {
            get;
            private set;
        }

        public AddZone(string name, ZoneInitialiser zone) {
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

