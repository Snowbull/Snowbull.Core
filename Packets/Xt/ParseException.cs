using System;

namespace Snowbull.API.Packets.Xt {
    public class ParseException : SnowbullException {
        public ParseException() {}
        public ParseException(string message) : base(message) {}
        public ParseException(string message, Exception inner) : base(message, inner) {}
    }
}

