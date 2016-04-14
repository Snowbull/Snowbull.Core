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
	sealed class ServerActor : ReceiveActor {
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        private readonly XmlMap xmlMap = API.Packets.PacketMapper.XmlMap();
        private readonly XtMap xtMap = API.Packets.PacketMapper.XtMap();
        private readonly Dictionary<string, IActorRef> zones = new Dictionary<string, IActorRef>();
		private readonly Server Observable;

		public static Props Props(string name) {
            return Akka.Actor.Props.Create(() => new ServerActor(name));
        }

		public ServerActor(string name) {
			Observable = new Server(name, Context);
            Receive<Tcp.Bound>(Bound);
            Receive<AddZone>(AddZone);
            Receive<Tcp.Connected>(Connected);
            Receive<Authenticate>(Authenticate);
            Receive<Disconnected>(Disconnected);
			API.Observer.Observer[] observers = API.Assemblies.Load("Plugins/").Get((API.IServer) Observable);
			IActorRef[] actors = new IActorRef[observers.Length];
			for(int i = 0; i < observers.Length; i++)
				actors[i] = Context.ActorOf(API.Observer.ObserverActor.Props(observers[i]));
        }

        private void Bound(Tcp.Bound bound) {
            logger.Info("Server bound to " + bound.LocalAddress);
        }

        private void AddZone(AddZone zone) {
            zones.Add(zone.Name, Context.ActorOf(zone.Zone));
        }

        private void Connected(Tcp.Connected connected) {
            logger.Info("New client at " + connected.RemoteAddress + " connected!");
            IActorRef connection = Context.ActorOf(ConnectionActor.Props(Self, Sender, connected.RemoteAddress, xmlMap, xtMap));
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

