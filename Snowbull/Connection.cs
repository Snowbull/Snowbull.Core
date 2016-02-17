using System;
using Akka;
using Akka.Actor;
using Akka.IO;
using System.Text;
using Akka.Event;
using XmlMap = System.Collections.Immutable.ImmutableDictionary<string, System.Func<System.Xml.XmlDocument, Snowbull.API.Packets.Xml.XmlPacket>>;
using System.Xml;

namespace Snowbull {
    public class Connection : ReceiveActor {
        private readonly IActorRef server;
        private readonly IActorRef socket;
        private readonly IActorRef user;
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        private readonly XmlMap xmlMap;
        private string buffer = "";

        public static Props Props(IActorRef server, IActorRef socket, XmlMap xmlMap) {
            return Akka.Actor.Props.Create(() => new Connection(server, socket, xmlMap));
        }

        public Connection(IActorRef server, IActorRef socket, XmlMap xmlMap) {
            this.server = server;
            this.socket = socket;
            this.xmlMap = xmlMap;
            user = Context.ActorOf(User.Props(Self, server));
            Receive<Tcp.Received>(Received);
            Receive<Tcp.PeerClosed>(Closed);
            Receive<API.Packets.ISendPacket>(Send);
        }

        private void Received(Tcp.Received received) {
            string recv = Encoding.UTF8.GetString(received.Data.ToArray());
            string[] data = (buffer + recv).Split('\0');
            for(int i = 0; i < data.Length - 1; i++) {
                string packet = data[i];
                #if DEBUG
                logger.Debug("Received: " + packet);
                #endif
                if(packet.StartsWith("<")) { // Better than throwing it at a parser to determine it.
                    ProcessXml(packet);
                }else if(packet.StartsWith("%")) {

                }
            }
            buffer = data[data.Length - 1];
        }

        private void ProcessXml(string xml) {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Prohibit;
            settings.XmlResolver = null;
            using(System.IO.StringReader sreader = new System.IO.StringReader(xml)) {
                using(XmlReader xreader = XmlReader.Create(sreader, settings)) {
                    XmlDocument document = new XmlDocument();
                    document.Load(xreader);
                    XmlElement element = document.DocumentElement;
                    XmlNode body = API.Packets.Xml.XmlMessage.Verify(element);
                    string action = body.Attributes["action"].Value;
                    API.Packets.Xml.XmlPacket packet = xmlMap[action](document);
                    user.Tell(packet, Self);
                }
            }
        }

        private void Send(API.Packets.ISendPacket packet) {
            #if DEBUG
            logger.Debug("Sending: " + packet);
            #endif
            byte[] bytes = Encoding.UTF8.GetBytes(packet + "\0");
            ByteStringBuilder bsb = new ByteStringBuilder();
            bsb.Append(bytes);
            socket.Tell(Tcp.Write.Create(bsb.Result()));
        }

        private void Closed(Tcp.PeerClosed closed) {
            logger.Info("Disconnected...");
            Become(Disconnected);
            server.Tell(new Disconnected(Self));
        }

        private void Disconnected() {
            
        }
    }
}

