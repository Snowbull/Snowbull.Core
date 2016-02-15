using System;

namespace Snowbull.API.Packets.Receive.Xml {
    public abstract class XmlPacket : Packet {
        public System.Xml.XmlElement Xml {
            get;
            private set;
        }

        protected XmlPacket(System.Xml.XmlElement xml) {
            Xml = xml;
        }

        public override string ToString() {
            return Xml.ToString();
        }
    }
}

