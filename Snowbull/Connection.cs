using System;
using Akka;
using Akka.Actor;
using Akka.IO;
using System.Text;
using Akka.Event;

namespace Snowbull {
    public class Connection : ReceiveActor {
        private readonly IActorRef server;
        private readonly IActorRef socket;
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);

        public static Props Props(IActorRef server, IActorRef socket) {
            return Akka.Actor.Props.Create(() => new Connection(server, socket));
        }

        public Connection(IActorRef server, IActorRef socket) {
            this.server = server;
            this.socket = socket;
            Receive<Tcp.Received>(Received);
            Receive<Send>(Send);
            Receive<Tcp.PeerClosed>(Closed);
            ReceiveAny(o => Console.WriteLine(o.ToString()));
        }

        private void Received(Tcp.Received received) {
            string packet = Encoding.UTF8.GetString(received.Data.ToArray());
            #if DEBUG
            logger.Debug("Received: " + packet);
            #endif
        }

        private void Send(Send send) {
            socket.Tell(Tcp.Write.Create(send.Data));
        }

        private void Closed(Tcp.PeerClosed closed) {
            logger.Info("Disconnected...");
            Become(Disconnected);
            server.Tell(new Disconnected(Self));
        }

        private void Disconnected() {
            
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

