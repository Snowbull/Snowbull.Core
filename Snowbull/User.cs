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
        private readonly string rndk = API.Cryptography.Random.GenerateRandomKey(10);

        public static Props Props(IActorRef connection, IActorRef server) {
            return Akka.Actor.Props.Create(() => new User(connection, server));
        }

        public User(IActorRef connection, IActorRef server) {
            this.connection = connection;
            this.server = server;
            BecomeStacked(APICheck);
        }

        private void APICheck() {
            Receive<VersionCheck>(VersionCheck);
        }

        private void KeyAgreement() {
            Receive<RandomKey>(RandomKey);
        }

        private void Authentication() {
            Receive<Login>(Login);
        }

        private void VersionCheck(VersionCheck verChk) {
            connection.Tell(API.Packets.Xml.Send.Authentication.ApiOK.Create());
            UnbecomeStacked();
            BecomeStacked(KeyAgreement);
        }

        private void RandomKey(RandomKey rndk) {
            connection.Tell(API.Packets.Xml.Send.Authentication.RandomKey.Create(this.rndk));
            UnbecomeStacked();
            BecomeStacked(Authentication);
        }

        private void Login(Login login) {
            logger.Debug(login.Username + ":" + login.Password);
        }
    }
}

