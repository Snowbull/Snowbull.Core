using System;
using Akka;
using Akka.Actor;
using Akka.Event;
using Snowbull.API.Packets.Xml.Receive.Authentication;
using System.Data.Entity;

namespace Snowbull {
	abstract class UserActor : API.Observer.ObservableActor {
        protected readonly IActorRef connection;
        protected readonly IActorRef zone;
        protected readonly IActorRef server;
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        protected readonly int id;
        protected readonly string username;

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.User"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="server">Server.</param>
        /// <param name="zone">The zone the user belongs to.</param> 
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username.</param>  
		protected UserActor(int id, string username, Func<string, IActorRef, API.Observer.Observable> creator, IActorRef connection, IActorRef zone, IActorRef server) : base(username, creator) {
            this.id = id;
            this.username = username;
            this.connection = connection;
            this.zone = zone;
            this.server = server;
        }

        protected override void PreStart() {
            zone.Tell(new UserInitialised(connection, Self, username));
        }

        protected override void PostStop() {
            zone.Tell(new UserStopped(connection, Self));
        }
    }
}

