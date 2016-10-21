using System;
using Akka.Actor;
using Snowbull.Core.Packets.Xt.Receive;
using Snowbull.Core.Packets.Xt.Receive.Authentication;
using Snowbull.Core.Packets.Xt.Receive.Player.EPF;
using Snowbull.Core.Packets.Xt.Receive.Player.Inventory;
using Snowbull.Core.Packets.Xt.Receive.Player.Relations.Buddies;
using Snowbull.Core.Packets.Xt.Receive.Player.Relations.Ignore;

namespace Snowbull.Core.Game {
	public class GameUserActor : UserActor {
		public static Props Props(GameUser user) {
			return Akka.Actor.Props.Create(() => new GameUserActor(user));
		}

		public GameUserActor(GameUser user) : base(user) {
		}

		protected override void Running() {
			base.Running();
			Receive<Packets.Xt.Receive.Authentication.JoinServer>(new Action<Packets.Xt.Receive.Authentication.JoinServer>(JoinServer));
            Receive<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(new Action<Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies>(GetBuddies));
            Receive<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(new Action<Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored>(GetIgnored));
            Receive<Packets.Xt.Receive.Player.Inventory.GetInventory>(new Action<Packets.Xt.Receive.Player.Inventory.GetInventory>(GetInventory));
            Receive<Packets.Xt.Receive.GetLastRevision>(new Action<Packets.Xt.Receive.GetLastRevision>(GetLastRevision));
            Receive<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(new Action<Packets.Xt.Receive.Player.EPF.GetEPFPoints>(GetEPFPoints));
		}

		private void JoinServer(Packets.Xt.Receive.Authentication.JoinServer js) {
			// We'll check the login key again I GUESS
			// But for now let's just accept it.
			connection.ActorRef.Tell(
				new Packets.Xt.Send.Authentication.JoinServer(
					agent: true, 
					guide: true,
					moderator: false, 
					modifiedStampCover: true
				)
			, Self);
		}

        private void GetBuddies(Packets.Xt.Receive.Player.Relations.Buddies.GetBuddies gb) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.Relations.Buddies.GetBuddies() // TODO - Load actual buddy list.
            , Self);
        }

        private void GetIgnored(Packets.Xt.Receive.Player.Relations.Ignore.GetIgnored gn) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.Relations.Ignore.GetIgnored() // TODO - Load actual ignore list.
            , Self);
        }

        private void GetInventory(Packets.Xt.Receive.Player.Inventory.GetInventory gi) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.Inventory.GetInventory() // TODO - Load actual inventory list.
            , Self);
        }

        private void GetLastRevision(Packets.Xt.Receive.GetLastRevision glr) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.GetLastRevision(3239)
            , Self);
        }

        private void GetEPFPoints(Packets.Xt.Receive.Player.EPF.GetEPFPoints epfgr) {
            connection.ActorRef.Tell(
                new Packets.Xt.Send.Player.EPF.GetEPFPoints(0, 1) // TODO - Find out what these are?
            , Self);
        }
	}
}

