using System;

namespace Snowbull.Core.Packets.Xt.Receive.Player {
    public class Frame : XtPacket, IReceivePacket {
        public int Id {
            get;
            private set;
        }

        public Frame(XtData xt) : base(xt) {
            Id = int.Parse(xt.Arguments[0]);
        }
    }
}

