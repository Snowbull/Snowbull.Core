using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Akka.Actor;
using Akka.Event;

namespace Snowbull {
    public abstract class Zone : ReceiveActor {
        private readonly Dictionary<IActorRef, IActorRef> users = new Dictionary<IActorRef, IActorRef>();
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        protected readonly IActorRef server;

        protected ReadOnlyDictionary<IActorRef, IActorRef> Users {
            get {
                return new ReadOnlyDictionary<IActorRef, IActorRef>(users);
            }
        }

        protected Zone(IActorRef server) {
            this.server = server;
            BecomeStacked(Running);
        }

        protected virtual void Running() {
            Receive<Authenticate>(Authenticate);
            Receive<UserInitialised>(UserInitialised);
            Receive<UserStopped>(UserStopped);
        }

        protected abstract void Authenticate(Authenticate authenticate);

        private void UserInitialised(UserInitialised ui) {
            users.Add(ui.Connection, ui.User);
        }

        private void UserStopped(UserStopped us) {
            users.Remove(us.Connection);
        }
    }

    public class Authenticate {
        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public IActorRef Sender {
            get;
            private set;
        }

        /// <summary>
        /// Gets the login request.
        /// </summary>
        /// <value>The login request.</value>
        public API.Packets.Xml.Receive.Authentication.Login Request {
            get;
            private set;
        }

        public string Key {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.Authenticate"/> class.
        /// </summary>
        /// <param name="request">Request.</param>
        /// <param name="sender">Sender.</param>
        /// <param name="key">Random key.</param> 
        public Authenticate(API.Packets.Xml.Receive.Authentication.Login request, IActorRef sender, string key) {
            Request = request;
            Sender = sender;
            Key = key;
        }
    }

    public class UserInitialised {
        public IActorRef Connection {
            get;
            private set;
        }

        public IActorRef User {
            get;
            private set;
        }

        public UserInitialised(IActorRef connection, IActorRef user) {
            Connection = connection;
            User = user;
        }
    }

    public class UserStopped {
        public IActorRef Connection {
            get;
            private set;
        }

        public IActorRef User {
            get;
            private set;
        }

        public UserStopped(IActorRef connection, IActorRef user) {
            Connection = connection;
            User = user;
        }
    }
}

