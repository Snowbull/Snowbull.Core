using System;
using System.Xml;
namespace Snowbull.API.Packets.Xml {
    public abstract class XmlPacket : IPacket {
        private XmlDocument Document {
            get;
            set;
        }

        protected XmlPacket(XmlDocument xml) {
            Document = xml;
        }

        public override string ToString() {
            return Document.DocumentElement.OuterXml;
        }
    }
}

