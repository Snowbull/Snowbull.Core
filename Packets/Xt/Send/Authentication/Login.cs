using System;

namespace Snowbull.API.Packets.Xt.Send.Authentication {
    public class Login : XtPacket, ISendPacket {
        public Login(int id, string key, string populations) : base(new XtData(From.Server, "l", new string[] { id.ToString(), key, "", populations })) {
        }

        public Login() : base(new XtData(From.Server, "l")) {
        }
    }
}

