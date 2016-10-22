namespace Snowbull.Core.Packets.Xt.Receive.Rooms {
    public class JoinRoom : XtPacket, IReceivePacket {
        public int ExternalID {
            get;
            private set;
        }

        public JoinRoom(XtData xt) : base(xt) {
            ExternalID = int.Parse(xt.Arguments[0]);
        }
    }
}

