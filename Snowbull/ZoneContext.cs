using System;
using Akka.Actor;

namespace Snowbull {
	public class ZoneContext : API.IZone {
		private IActorRef actor;

		public string Name {
			get;
			private set;
		}

		public ZoneContext(string name, IActorRef actor) {
			Name = name;
			this.actor = actor;
		}

		public void Send(API.Packets.ISendPacket packet) {
			actor.Tell(packet);
		}
	}
}

