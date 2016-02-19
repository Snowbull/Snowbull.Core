using System;
using Akka.Actor;
using System.Net;
using Akka.IO;
using System.Collections.Generic;

namespace Snowbull {
    public delegate Props ZoneInitialiser(IActorRef server);

    public class Snowbull {
        private readonly ActorSystem actors = ActorSystem.Create("Snowbull");
        private readonly IActorRef actor;

        public IActorRef Actor {
            get {
                return actor;
            }
        }

        public Snowbull(Dictionary<string, ZoneInitialiser> zones) {
            actor = actors.ActorOf(Server.Props());
            foreach(KeyValuePair<string, ZoneInitialiser> zone in zones)
                actor.Tell(new AddZone(zone.Key, zone.Value(actor)));
        }

        public void Bind(IPAddress host, int port) {
            if(host == null) throw new ArgumentNullException("host");
            actors.Tcp().Tell(new Tcp.Bind(actor, new IPEndPoint(host, port)), actor);
        }
    }
}

