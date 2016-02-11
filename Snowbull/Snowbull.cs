using System;
using Akka.Actor;
using System.Net;
using Akka.IO;

namespace Snowbull {
    public class Snowbull {
        private readonly ActorSystem actors = ActorSystem.Create("Snowbull");
        private readonly IActorRef server;

        public Snowbull() {
            server = actors.ActorOf(Server.Props());
        }

        public void Bind(IPAddress host, int port) {
            if(host == null) throw new ArgumentNullException("host");
            actors.Tcp().Tell(new Tcp.Bind(server, new IPEndPoint(host, port)));
        }
    }
}

