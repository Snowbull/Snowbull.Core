using System;

namespace Snowbull.API.Packets.Xml.Send.Policy {
	internal sealed class Allow : Rule {
		public Allow() : base("allow-access-from") {
		}

		public Allow(string host) : base("allow-access-from", host) {
		}

		public Allow(string host, int port) : base("allow-access-from", host, port) {
		}
	}
}

