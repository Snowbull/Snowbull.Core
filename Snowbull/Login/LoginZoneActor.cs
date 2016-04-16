using System;
using Akka.Actor;
using System.Data.Entity;

namespace Snowbull.Login {
    sealed class LoginZoneActor : ZoneActor {

		public static Props Props(string name, IActorRef server, IActorRef oparent) {
			return Akka.Actor.Props.Create(() => new LoginZoneActor(name, server, oparent));
        }

		public LoginZoneActor(string name, IActorRef server, IActorRef oparent) : base(name, (n, a) => new LoginZone(n, a, oparent), server) {
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
					connection.Tell(new Authenticated(LoginUserActor.Props(auth.Credentials.Id, auth.Credentials.Username, connection, Self, server, Observable.Actor), auth.Credentials), Self);
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
}

