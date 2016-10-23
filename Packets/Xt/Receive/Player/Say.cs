using System;

namespace Snowbull.Core.Packets.Xt.Receive.Player {
    public class Say : XtPacket, IReceivePacket {
        public string Message {
            get;
            private set;
        }

        public Say(XtData xt) : base(xt) {
            Message = xt.Arguments[1];
        }
    }
}

