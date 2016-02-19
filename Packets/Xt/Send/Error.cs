using System;

namespace Snowbull.API.Packets.Xt.Send {
    public class Error : XtPacket, ISendPacket {
        public Error Type {
            get;
            private set;
        }

        public Error(Errors error, int room) : base(new XtData(From.Server, "e", new [] { ((int) error).ToString() }, room)) {
            
        }
    }
}

