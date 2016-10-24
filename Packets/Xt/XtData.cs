/**
 * Xt Parser for Snowbull's Plugin API ("Snowbull").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull".
 * 
 * "Snowbull" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Snowbull.Core.Packets.Xt {
    public enum From {
        Client,
        Server
    }

    public sealed class XtData {
        /// <summary>
        /// Gets the sender.
        /// </summary>
        /// <value>The sender.</value>
        public From Sender {
            get;
            private set;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public string Command {
            get;
            private set;
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public ImmutableArray<string> Arguments {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Room.
        /// </summary>
        /// <value>The Room.</value>
        public int Room {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.XtData"/> class.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="command">Command.</param>
        /// <param name="room">Room.</param>
        /// <param name="arguments">Arguments.</param>
        public XtData(From sender, string command, string[] arguments = null, int room = -1) {
            if(command == null) throw new ArgumentNullException("command");
            if(arguments == null) arguments = new string[] {};
            Sender = sender;
            Command = command;
            Room = room;
            Arguments = arguments.ToImmutableArray();
        }

        public static XtData Parse(string data, From sender) {
            if(data == null) throw new ArgumentNullException("data");
            string command;
            int room;
            string[] args;
            string regex = sender == From.Client ? @"^(%xt%[s|z]%[a-zA-Z0-9#]*%[-.0-9]*%.*?)$" : @"^(%xt%[a-z|A-Z|0-9|#|_]*%-?[0-9]*%.*?)$";
            int cmdp = sender == From.Client ? 3 : 2;
            int roomp = sender == From.Client ? 4 : 3;
            int argp = sender == From.Client ? 5 : 4;
            if(!Regex.Match(data, regex).Success)
                throw new ParseException("The packet given is not a valid " + sender.ToString().ToLower() + " xt packet!");
            string[] parts = data.Split('%');
            if(parts.Length > cmdp)
                command = parts[cmdp];
            else
                throw new ParseException("Packet has no command.");
            if(parts.Length > roomp)
                room = int.Parse(parts[roomp]);
            else
                throw new ParseException("Packet has no Room.");
            if(parts.Length - 1 > argp) {
                args = new string[parts.Length - 1 - argp];
                for(int i = argp; i < parts.Length - 1; i++)
                    args[i - argp] = parts[i];
            }else{
                args = new string[0];
            }
            return new XtData(sender, command, args, room);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Snowbull.XtData"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Snowbull.XtData"/>.</returns>
        public override string ToString() {
            string data = "%xt%" + (Sender == From.Client ? "s" : Command) + "%" + Room + (Sender == From.Client ? "%" + Command : "");
            foreach(string arg in Arguments) data += "%" + arg;
            data += "%";
            return data;
        }


    }
}

