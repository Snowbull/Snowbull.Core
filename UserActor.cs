using System;
using Akka.Actor;
using Akka.Persistence;
using Akka.Event;

namespace Snowbull.Core {
    public abstract class UserActor : ReceivePersistentActor {
		protected readonly User user;
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);

        public override string PersistenceId {
            get;
        }

		protected int Id {
			get { return user.Id; }
		}

		protected string Username {
			get { return user.Username; }
		}

        protected Connection Connection {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.User"/> class.
        /// </summary>
        /// <param name="connection">Connection.</param>
        /// <param name="server">Server.</param>
        /// <param name="zone">The zone the user belongs to.</param> 
        /// <param name="id">The user's id.</param>
        /// <param name="username">The user's username.</param>  
        public UserActor(User user, string persistenceId) : base() {
			this.user = user;
            PersistenceId = persistenceId;
			Connection = (Connection) user.Connection;
            Context.Watch(Connection.ActorRef);
			BecomeStacked(Running);
        }

		protected virtual void Running() {
            Command<Packets.ISendPacket>(new Action<Packets.ISendPacket>(Send));
            Command<Terminated>(t => { if(t.ActorRef == Connection.ActorRef) Context.Stop(Self); });
		}

        private void Send(Packets.ISendPacket packet) {
            // Forward on any packets sent to the user.
            Connection.ActorRef.Forward(packet);
        }
    }
}

