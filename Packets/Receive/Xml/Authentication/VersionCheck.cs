using System;
using System.Xml;

namespace Snowbull.API.Packets.Receive.Xml.Authentication {
    public class VersionCheck : XmlMessage {
        public int Version {
            get;
            private set;
        }

        public VersionCheck(XmlElement xml) : base(xml) {
            if(!Body.HasChildNodes) throw new MessageParseException("Message body has no child nodes to read.", xml);
            XmlNode ver = null;
            foreach(XmlNode node in Body.ChildNodes)
                if(node.Name == "ver") ver = node;
            if(ver == null) throw new MessageParseException("Message body has no ver node to read.", xml);
            if(ver.Attributes["v"] == null) throw new MessageParseException("Version node has no version number.", xml);
            int version;
            if(!int.TryParse(ver.Attributes["v"].Value, out version)) throw new MessageParseException("Version number is not an integer.", xml);
            Version = version;
        }
    }
}

