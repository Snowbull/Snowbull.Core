using System;

namespace Snowbull.Core.Packets.Xt.Send.Player {
    public class Move : XtPacket, ISendPacket {
        public Move(Game.Player.Player player, int room) : base(
            new XtData(
                From.Server,
                "sp",
                new string[] { player.User.Id.ToString(), player.Position.X.ToString(), player.Position.Y.ToString() },
                room
            )
        ) {}
    }
}

