using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Snowbull.API.Packets.Receive.Xt {
    public enum From {
        Client,
        Server
    }

    public class Parser {
        private readonly string data;

        public string Command {
            get;
            private set;
        }

        public ImmutableArray<string> Arguments {
            get;
            private set;
        }

        public int Room {
            get;
            private set;
        }

        public Parser(string data, From sender) {
            if(data == null) throw new ArgumentNullException("data");
            this.data = data;
            string regex = sender == From.Client ? @"^(%xt%[s|z]%[a-zA-Z0-9#]*%[-.0-9]*%.*?)$" : @"^(%xt%[a-z|A-Z|0-9|#|_]*%-?[0-9]*%.*?)$";
            int cmdp = sender == From.Client ? 3 : 2;
            int roomp = sender == From.Client ? 4 : 3;
            int argp = sender == From.Client ? 5 : 4;
            string[] args = new string[] {};
            if(!Regex.Match(data, regex).Success)
                throw new ParseException("The packet given is not a valid " + sender.ToString().ToLower() + " xt packet!");
            string[] parts = data.Split('%');
            if(parts.Length > cmdp)
                Command = parts[cmdp];
            else
                throw new ParseException("Packet has no command.");
            if(parts.Length > roomp)
                Room = int.Parse(parts[roomp]);
            else
                throw new ParseException("Packet has no room.");
            if(parts.Length > argp) {
                args = new string[parts.Length - argp];
                for(int i = argp; i < parts.Length; i++)
                    args[i-argp] = parts[argp];
            }
            Arguments = args.ToImmutableArray<string>();
        }

        public override string ToString() {
            return data;
        }
    }
}

