using System;
using System.Xml;

namespace Snowbull.API.Packets.Xml {
    public class MessageParseException : SnowbullException {

        public MessageParseException() : base() {}

        public MessageParseException(string message) : base(message) {}

        public MessageParseException(string message, Exception inner) : base(message, inner) {}
    }
}

