using System;
using Akka.Actor;

namespace Snowbull.Login {
    sealed class LoginUserActor : UserActor {
        /// <summary>
        /// Creates a user actor with id, username, connection, zone and server props.
        /// </summary>
        /// <param name="connection">The user's connection.</param>
        /// <param name="server">The server the user is connected to.</param>
        /// <param name="zone">The zone the user is in.</param> 
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username.</param>  
        public static Props Props(int id, string username, IActorRef connection, IActorRef zone, IActorRef server) {
            return Akka.Actor.Props.Create(() => new LoginUserActor(id, username, connection, zone, server));
        }

		public LoginUserActor(int id, string username, IActorRef connection, IActorRef zone, IActorRef server) : base(id, username, (n, a) => new LoginUser(n, a), connection, zone, server) {
        }

        protected override void PreStart() {
            base.PreStart();
            connection.Tell(new API.Packets.Xt.Send.Authentication.Login(id, "", ""));
        }
    }
}

