using System;
using Akka;
using Akka.IO;
using Akka.Actor;
using System.Net;
using Akka.Event;

namespace Snowbull {
    public class Server : ReceiveActor {
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);

        public static Props Props(IPAddress host, int port) {
            return Akka.Actor.Props.Create(() => new Server(host, port));
        }

        public Server(IPAddress host, int port) {
            Receive<Tcp.Bound>(Bound);
            Receive<Tcp.Connected>(Connected);
            Receive<Disconnected>(Disconnected);
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(host, port)));
        }

        private void Bound(Tcp.Bound bound) {
            logger.Info("Server bound to " + bound.LocalAddress);
        }

        private void Connected(Tcp.Connected connected) {
            logger.Info("New client at " + connected.RemoteAddress + " connected!");
            IActorRef connection = Context.ActorOf(Connection.Props(Self, Sender));
            Sender.Tell(new Tcp.Register(connection));
        }

        private void Disconnected(Disconnected disconnected) {
            Context.Stop(disconnected.Actor);
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

