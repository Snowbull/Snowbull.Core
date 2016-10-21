namespace Snowbull.Core.Packets.Xt.Send.Rooms {
    public class RemovePlayer : XtPacket, ISendPacket {
        public RemovePlayer(int id, int room) : base(
            new XtData(
                From.Server,
                "rp",
                new string[] { id.ToString() },
                room
            )
        ) {}
    }
}

