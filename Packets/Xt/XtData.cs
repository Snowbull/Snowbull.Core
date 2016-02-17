using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Snowbull.API.Packets.Xt {
    public enum From {
        Client,
        Server
    }

    public class XtData {
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
        /// Gets the room.
        /// </summary>
        /// <value>The room.</value>
        public int Room {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Snowbull.API.XtData"/> class.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="command">Command.</param>
        /// <param name="room">Room.</param>
        /// <param name="arguments">Arguments.</param>
        public XtData(From sender, string command, int room, string[] arguments = null) {
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
            string[] args = new string[] {};
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
                throw new ParseException("Packet has no room.");
            if(parts.Length > argp) {
                args = new string[parts.Length - argp];
                for(int i = argp; i < parts.Length; i++)
                    args[i-argp] = parts[argp];
            }
            return new XtData(sender, command, room, args);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current <see cref="Snowbull.API.XtData"/>.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the current <see cref="Snowbull.API.XtData"/>.</returns>
        public override string ToString() {
            string data = "%xt%" + (Sender == From.Client ? "s" : Command) + "%" + Room + (Sender == From.Client ? "%" + Command : "");
            foreach(string arg in Arguments) data += "%" + arg;
            data += "%";
            return data;
        }


    }
}

