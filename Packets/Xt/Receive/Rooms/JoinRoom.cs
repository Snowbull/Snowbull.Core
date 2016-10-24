namespace Snowbull.Core.Packets.Xt.Receive.Rooms {
    public class JoinRoom : XtPacket, IReceivePacket {
        public int ExternalId {
            get;
            private set;
        }

        public int X {
            get;
            private set;
        }

        public int Y {
            get;
            private set;
        }

        public JoinRoom(XtData xt) : base(xt) {
            ExternalId = int.Parse(xt.Arguments[0]);
            int x = 0, y = 0;
            if(xt.Arguments.Length >= 3) {
                int.TryParse(xt.Arguments[1], out x);
                int.TryParse(xt.Arguments[2], out y);
            }
            X = x;
            Y = y;
        }
    }
}

