using System;

namespace Snowbull.Core.Packets.Xt.Receive.Player {
    public class Move : XtPacket, IReceivePacket {
        public int X {
            get;
            private set;
        }

        public int Y {
            get;
            private set;
        }

        public Move(XtData xt) : base(xt) {
            X = int.Parse(xt.Arguments[0]);
            Y = int.Parse(xt.Arguments[1]);
        }
    }
}

