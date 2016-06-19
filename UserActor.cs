using System;
using Akka;
using Akka.Actor;
using Akka.Event;
using Snowbull.Packets.Xml.Receive.Authentication;
using System.Data.Entity;

namespace Snowbull {
	public abstract class UserActor : SnowbullActor {
		protected readonly User user;
		protected readonly Connection connection;
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);

		protected int Id {
			get { return user.Id; }
		}

		protected string Username {
			get { return user.Username; }
		}



        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.User"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="server">Server.</param>
        /// <param name="zone">The zone the user belongs to.</param> 
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username.</param>  
		public UserActor(User user) : base() {
			this.user = user;
			connection = (Connection) user.Connection;
			BecomeStacked(Running);
        }

		protected virtual void Running() {
		}
    }
}

