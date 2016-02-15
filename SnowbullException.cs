using System;
using System.Runtime.Serialization;

namespace Snowbull.API {
    public abstract class SnowbullException : Exception {
        protected SnowbullException() : base() {}
        protected SnowbullException(string message) : base(message) {}
        protected SnowbullException(string message, Exception inner) : base(message, inner) {}
        protected SnowbullException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }
}

