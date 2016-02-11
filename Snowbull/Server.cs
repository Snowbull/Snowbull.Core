using System;
using Akka;
using Akka.IO;
using Akka.Actor;
using System.Net;

namespace Snowbull {
    public class Server : ReceiveActor {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); //< log4net logger.

        public static Props Props() {
            return Akka.Actor.Props.Create(() => new Server());
        }

        public Server() {
            Receive<Tcp.Bound>(Bound);
            Receive<Tcp.Connected>(Connected);
        }

        private void Bound(Tcp.Bound bound) {
            Console.WriteLine("Server bound to " + bound.LocalAddress);
            logger.Info("Server bound to " + bound.LocalAddress);
        }

        private void Connected(Tcp.Connected connected) {
            logger.Info("New client at " + connected.RemoteAddress + " connected!");
            IActorRef connection = Context.ActorOf(Connection.Props(Sender));
            Sender.Tell(new Tcp.Register(connection));
        }

    }
}

