using System.Collections.Immutable;

namespace Snowbull.Core.Packets.Xt.Send.Rooms {
    public class JoinedRoom : XtPacket, ISendPacket {
        public JoinedRoom(int externalId, ImmutableList<Game.Player.Player> players, int room) : base(
            new XtData(
                From.Server,
                "jr",
                new string[] { externalId.ToString(), PlayersToString(players) },
                room
            )
        ) {}

        private static string PlayersToString(ImmutableList<Game.Player.Player> players) {
            string data = "";
            for(int i = 0; i < players.Count; i++) {
                Game.Player.Player player = players[i];
                data += player.ToString() + ((i + 1) != players.Count ? "%" : "");
            }
            return data;
        }
    }
}

