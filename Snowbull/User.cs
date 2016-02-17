using System;
using Akka;
using Akka.Actor;
using Akka.Event;
using Snowbull.API.Packets.Xml.Receive.Authentication;

namespace Snowbull {
    public class User : ReceiveActor {
        private readonly IActorRef connection;
        private readonly IActorRef server;
        private readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        private readonly string key = API.Cryptography.Random.GenerateRandomKey(10);

        /// <summary>
        /// Creates a user actor with connection/server props.
        /// </summary>
        /// <param name="connection">The user's connection.</param>
        /// <param name="server">The server the user is connected to.</param>
        public static Props Props(IActorRef connection, IActorRef server) {
            return Akka.Actor.Props.Create(() => new User(connection, server));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.User"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="server">Server.</param>
        public User(IActorRef connection, IActorRef server) {
            this.connection = connection;
            this.server = server;
            BecomeStacked(APICheck);
        }

        /// <summary>
        /// Sets the actor to expect an API version check.
        /// </summary>
        private void APICheck() {
            Receive<VersionCheck>(VersionCheck);
        }

        /// <summary>
        /// Sets the actor to expect a random key request.
        /// </summary>
        private void KeyAgreement() {
            Receive<RandomKey>(RandomKey);
        }

        /// <summary>
        /// Sets the actor to expect an authentication packet.
        /// </summary>
        private void Authentication() {
            Receive<Login>(Login);
        }

        /// <summary>
        /// Checks the client's API version.
        /// </summary>
        /// <param name="verChk">Version check packet.</param>
        private void VersionCheck(VersionCheck verChk) {
            // Tell the client that the version is fine.
            connection.Tell(API.Packets.Xml.Send.Authentication.ApiOK.Create());
            // Expect random key request.
            UnbecomeStacked();
            BecomeStacked(KeyAgreement);
        }

        /// <summary>
        /// Handles a request for a random key.
        /// </summary>
        /// <param name="rndk">Random key request packet.</param>
        private void RandomKey(RandomKey rndk) {
            // Tell the client the random key.
            connection.Tell(API.Packets.Xml.Send.Authentication.RandomKey.Create(key));
            // Expect a login request.
            UnbecomeStacked();
            BecomeStacked(Authentication);
        }

        /// <summary>
        /// Handles a login request.
        /// </summary>
        /// <param name="login">Login request packet.</param>
        private void Login(Login login) {
            logger.Debug(login.Username + ":" + login.Password);
        }
    }
}

