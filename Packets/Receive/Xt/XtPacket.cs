using System;

namespace Snowbull.API.Packets.Receive.Xt {
    public abstract class XtPacket : Packet {
        public Parser Xt {
            get;
            private set;
        }

        protected XtPacket(Parser xt) {
            Xt = xt;
        }

        public override string ToString() {
            return Xt.ToString();
        }
    }
}

