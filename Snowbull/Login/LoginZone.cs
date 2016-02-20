using System;
using Akka.Actor;
using System.Data.Entity;

namespace Snowbull.Login {
    public class LoginZone : Zone {
        public static Props Props(IActorRef server) {
            return Akka.Actor.Props.Create(() => new LoginZone(server));
        }

        public LoginZone(IActorRef server) : base(server) {
        }

        protected override void Running() {
            base.Running();
            Receive<Authentication>(Authentication);
        }

        protected override void Authenticate(Authenticate auth) {
            Data.SnowbullContext db = new Data.SnowbullContext();
            db.Users.FirstAsync<Data.Models.User>(u => u.Username == auth.Request.Username).ContinueWith<Authentication>(
                t => {
                    Data.Models.Immutable.ImmutableCredentials credentials = t.IsFaulted ? null : new Data.Models.Immutable.ImmutableCredentials(t.Result);
                    return new Authentication(credentials, auth);
                }
            ).PipeTo(Self);
        }

        private void Authentication(Authentication auth) {
            IActorRef connection = auth.Request.Sender;
            if(auth.Credentials != null) {
                string hash = API.Cryptography.Hashing.HashPassword(auth.Credentials.Password, auth.Request.Key);
                if(auth.Request.Request.Password == hash) {
                    logger.Debug("Authenticated as '" + auth.Credentials.Username + "'!");
                    connection.Tell(new Authenticated(LoginUser.Props(auth.Credentials.Id, auth.Credentials.Username, connection, Self, server)), Self);
                }else{
                    logger.Info("Failed to identify as '" + auth.Request.Request.Username + "'.");
                    connection.Tell(new API.Packets.Xt.Send.Error(API.Errors.PASSWORD_WRONG, -1), Self);
                    connection.Tell(new Disconnect());
                }
            }else{
                logger.Info("Attempt to login as non existent user '" + auth.Request.Request.Username + "'.");
                connection.Tell(new API.Packets.Xt.Send.Error(API.Errors.NAME_NOT_FOUND, -1), Self);
                connection.Tell(new Disconnect());
            }
        }
    }

    class Authentication {
        public Data.Models.Immutable.ImmutableCredentials Credentials {
            get;
            private set;
        }

        public Authenticate Request {
            get;
            private set;
        }

        public Authentication(Data.Models.Immutable.ImmutableCredentials credentials, Authenticate request) {
            Credentials = credentials;
            Request = request;
        }
    }
}

