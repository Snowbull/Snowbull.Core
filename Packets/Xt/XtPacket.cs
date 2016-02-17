using System;

namespace Snowbull.API.Packets.Xt {
    public abstract class XtPacket : IPacket {
        public XtData Xt {
            get;
            private set;
        }

        protected XtPacket(XtData xt) {
            Xt = xt;
        }

        public override string ToString() {
            return Xt.ToString();
        }
    }
}

