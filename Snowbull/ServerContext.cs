using System;

namespace Snowbull {
	public class ServerContext : API.IServer {
		public string Name {
			get;
			private set;
		}

		public ServerContext(string name) {
			Name = name;
		}
	}
}

