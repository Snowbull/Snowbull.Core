using System;
using Akka;
using Akka.Actor;
using Akka.IO;
using System.Text;
using System.Collections.Generic;

namespace Snowbull {
    public class Connection : ReceiveActor {
        private readonly IActorRef socket;
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType); //< log4net logger.

        public static Props Props(IActorRef socket) {
            return Akka.Actor.Props.Create(() => new Connection(socket));
        }

        public Connection(IActorRef socket) {
            this.socket = socket;
            Receive<Tcp.Received>(Received);
            Receive<Send>(Send);
            Receive<Tcp.ConnectionClosed>(Closed);
        }

        private void Received(Tcp.Received received) {
            string packet = Encoding.UTF8.GetString(received.Data.ToArray());
            #if DEBUG
            Console.WriteLine("Received: " + packet);
            logger.Debug("Received: " + packet);
            #endif
        }

        private void Send(Send send) {
            socket.Tell(Tcp.Write.Create(send.Data));
        }

        private void Closed(Tcp.ConnectionClosed closed) {
            Console.WriteLine("Disconnected.");
        }
    }

    public class Send {
        public ByteString Data {
            get;
            private set;
        }

        public Send(byte[] bytes) {
            ByteStringBuilder bsb = new ByteStringBuilder();
            bsb.Append(bytes);
            Data = bsb.Result();
        }
    }
}

