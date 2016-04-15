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
		public static Props Props(int id, string username, IActorRef connection, IActorRef zone, IActorRef server, IActorRef oparent) {
            return Akka.Actor.Props.Create(() => new LoginUserActor(id, username, connection, zone, server, oparent));
        }

		public LoginUserActor(int id, string username, IActorRef connection, IActorRef zone, IActorRef server, IActorRef oparent) : base(id, username, (n, a) => new LoginUser(n, a, oparent), connection, zone, server) {
        }

        protected override void PreStart() {
            base.PreStart();
			string key = API.Cryptography.Random.GenerateRandomKey(32); // This should be sent to remote actors with Akka.Remote.
			connection.Tell(new API.Packets.Xt.Send.Authentication.Login(id, key, ""));
        }
    }
}

