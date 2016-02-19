using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;

namespace Snowbull {
    public abstract class Zone : ReceiveActor {
        protected readonly Dictionary<IActorRef, IActorRef> users = new Dictionary<IActorRef, IActorRef>();
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        protected readonly IActorRef server;

        protected Zone(IActorRef server) {
            this.server = server;
            Receive<Authenticate>(Authenticate);
        }

        protected abstract void Authenticate(Authenticate authenticate);
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
}

