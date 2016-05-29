using System;
using System.Xml;

namespace Snowbull.API.Packets.Xml.Send.Authentication {
    public sealed class RandomKey: XmlMessage, ISendPacket {

        private RandomKey(XmlDocument document, XmlElement[] inner) : base(document, "rndK", -1, inner) {
        }

        public static RandomKey Create(string rndk) {
            XmlDocument document = new XmlDocument();
            XmlElement key = document.CreateElement("k");
            key.InnerText = rndk;
            return new RandomKey(document, new [] { key });
        }
    }
}

