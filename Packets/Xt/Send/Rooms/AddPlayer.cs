namespace Snowbull.Core.Packets.Xt.Send.Rooms {
    public class AddPlayer : XtPacket, ISendPacket {
        public AddPlayer(int id, string data, int room) : base(
            new XtData(
                From.Server,
                "ap",
                new string[] { id.ToString(), data },
                room
            )
        ) {}
    }
}

