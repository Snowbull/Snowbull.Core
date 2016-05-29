using System;

namespace Snowbull.API.Packets.Xml.Send.Policy {
	public abstract class Rule {
		internal string Tag {
			get;
			private set;
		}

		internal string Host {
			get;
			private set;
		}

		internal string Port {
			get;
			private set;
		}

		private Rule(string tag, string host, string port) {
			Tag = tag;
			Host = host;
			Port = port;
		}

		internal Rule(string tag, string host, int port) : this(tag, host, port.ToString()) {
		}

		internal Rule(string tag, string host) : this(tag, host, "*") {
		}

		internal Rule(string tag) : this(tag, "*", "*") {
		}
	}
}

