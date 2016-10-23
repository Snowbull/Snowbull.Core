using System;

namespace Snowbull.Core.Packets.Xt.Send.Player {
    public class Frame : XtPacket, ISendPacket {
        public Frame(Game.Player.Player player, int frame, int room) : base(
            new XtData(
                From.Server,
                "sf",
                new string[] { player.User.Id.ToString(), frame.ToString() },
                room
            )
        ) {}
    }
}

