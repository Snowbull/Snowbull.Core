using System;
using Akka.Actor;

namespace Snowbull.Login {
    public class LoginUser : User {
        /// <summary>
        /// Creates a user actor with id, username, connection, zone and server props.
        /// </summary>
        /// <param name="connection">The user's connection.</param>
        /// <param name="server">The server the user is connected to.</param>
        /// <param name="zone">The zone the user is in.</param> 
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username.</param>  
        public static Props Props(int id, string username, IActorRef connection, IActorRef zone, IActorRef server) {
            return Akka.Actor.Props.Create(() => new LoginUser(id, username, connection, zone, server));
        }

        public LoginUser(int id, string username, IActorRef connection, IActorRef zone, IActorRef server) : base(id, username, connection, zone, server) {
        }
    }
}

