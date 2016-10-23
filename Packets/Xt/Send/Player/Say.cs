using System;

namespace Snowbull.Core.Packets.Xt.Send.Player {
    public class Say : XtPacket, ISendPacket {
        public Say(Game.Player.Player player, string message, int room) : base(
            new XtData(
                From.Server,
                "m#sm",
                new string[] { player.User.Id.ToString(), message },
                room
            )
        ) {}
    }
}

