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
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;

namespace Snowbull.Core {
	abstract class ZoneActor : SnowbullActor {
        protected readonly ILoggingAdapter logger = Logging.GetLogger(Context);
		protected readonly Zone zone;

		public ZoneActor(Zone zone) {
			this.zone = zone;
            BecomeStacked(Running);
        }

        protected virtual void Running() {
			Receive<Authenticate>(new Action<Authenticate>(Authenticate));
			Receive<Authentication>(new Action<Authentication>(Authentication));
        }

        protected abstract void Authenticate(Authenticate authenticate);

		private void Authentication(Authentication auth) {
			Authentication(auth.Request, auth.Credentials);
		}

		protected abstract void Authentication(Authenticate request, Data.Models.Immutable.ImmutableCredentials credentials);
			
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

    class Authenticate {
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
        public Packets.Xml.Receive.Authentication.Login Request {
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
        public Authenticate(Packets.Xml.Receive.Authentication.Login request, Connection sender, string key) {
            Request = request;
            Sender = sender;
            Key = key;
        }
    }
}

