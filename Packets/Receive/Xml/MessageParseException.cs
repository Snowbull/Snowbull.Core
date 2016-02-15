using System;
using System.Xml;

namespace Snowbull.API.Packets.Receive.Xml {
    public class MessageParseException : SnowbullException {
        public XmlElement Xml {
            get;
            private set;
        }

        public MessageParseException(XmlElement xml) : base() {
            Xml = xml;
        }

        public MessageParseException(string message, XmlElement xml) : base(message) {
            Xml = xml;
        }

        public MessageParseException(string message, Exception inner, XmlElement xml) : base(message, inner) {
            Xml = xml;
        }
    }
}

