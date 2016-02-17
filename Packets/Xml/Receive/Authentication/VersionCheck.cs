using System;
using System.Xml;

namespace Snowbull.API.Packets.Xml.Receive.Authentication {
    public class VersionCheck : XmlMessage, IReceivePacket {
        public int Version {
            get;
            private set;
        }

        public VersionCheck(XmlDocument xml) : base(xml) {
            if(!Body.HasChildNodes) throw new MessageParseException("Message body has no child nodes to read.");
            XmlNode ver = null;
            foreach(XmlNode node in Body.ChildNodes)
                if(node.Name == "ver") ver = node;
            if(ver == null) throw new MessageParseException("Message body has no ver node to read.");
            if(ver.Attributes["v"] == null) throw new MessageParseException("Version node has no version number.");
            int version;
            if(!int.TryParse(ver.Attributes["v"].Value, out version)) throw new MessageParseException("Version number is not an integer.");
            Version = version;
        }
    }
}

