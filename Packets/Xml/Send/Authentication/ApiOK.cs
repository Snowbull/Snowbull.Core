using System;
using System.Xml;

namespace Snowbull.API.Packets.Xml.Send.Authentication {
    public class ApiOK : XmlMessage, ISendPacket {
        private ApiOK() : base(new XmlDocument(), "apiOK", 0, null) {
        }

        public static ApiOK Create() {
            return new ApiOK();
        }
    }
}

