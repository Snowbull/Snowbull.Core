/**
 * Zone Akka Actor for Snowbull.
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of Snowbull.
 * 
 * Snowbull is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * Snowbull is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Snowbull. If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Akka.Actor;
using Akka.Event;

namespace Snowbull {
	abstract class ZoneActor : SnowbullActor {
        private readonly Dictionary<IActorRef, User> users = new Dictionary<IActorRef, User>();
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);
        protected readonly Server server;

        protected ReadOnlyDictionary<IActorRef, User> Users {
            get {
                return new ReadOnlyDictionary<IActorRef, User>(users);
            }
        }

		public ZoneActor(string name, Func<string, IActorContext, API.Observer.Observable> creator, Server server) : base(creator(name, Context)) {
            this.server = server;
            BecomeStacked(Running);
        }

        protected virtual void Running() {
            Receive<Authenticate>(Authenticate);
            Receive<UserInitialised>(UserInitialised);
			Receive<API.Packets.ISendPacket>(SendPacket);
			Receive<Terminated>(Terminated);
        }

        protected abstract void Authenticate(Authenticate authenticate);

        private void UserInitialised(UserInitialised ui) {
			users.Add(ui.User.InternalActor, ui.User);
			Context.Watch(ui.User.InternalActor);
        }

		private void SendPacket(API.Packets.ISendPacket packet) {
			foreach(IActorRef user in users.Values)
				user.Forward(packet);
		}

		private void Terminated(Terminated t) {
			if(users.ContainsKey(t.ActorRef))
				users.Remove(t.ActorRef);
		}
    }

    internal class Authenticate {
        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public Connection Sender {
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
        public Authenticate(API.Packets.Xml.Receive.Authentication.Login request, Connection sender, string key) {
            Request = request;
            Sender = sender;
            Key = key;
        }
    }

    internal class UserInitialised {
		public Connection Connection {
            get;
            private set;
        }

        public User User {
            get;
            private set;
        }
		
		public string Username {
			get;
			private set;
		}

		public UserInitialised(Connection connection, User user, string username) {
            Connection = connection;
            User = user;
			Username = username;
        }
    }
}

