using System;

namespace Snowbull.Core.Packets.Xt.Receive.Player {
    public class Action : XtPacket, IReceivePacket {
        public int Id {
            get;
            private set;
        }

        public Action(XtData xt) : base(xt) {
            Id = int.Parse(xt.Arguments[0]);
        }
    }
}

