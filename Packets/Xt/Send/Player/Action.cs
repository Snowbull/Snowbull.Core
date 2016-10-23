using System;

namespace Snowbull.Core.Packets.Xt.Send.Player {
    public class Action : XtPacket, ISendPacket {
        public Action(Game.Player.Player player, int action, int room) : base(
            new XtData(
                From.Server,
                "sa",
                new string[] { player.User.Id.ToString(), action.ToString() },
                room
            )
        ) {}
    }
}

