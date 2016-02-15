using System;
using System.Xml;

namespace Snowbull.API.Packets.Receive.Xml {
    public class XmlMessage  : XmlPacket {
        public string Action {
            get;
            private set;
        }

        public XmlNode Body {
            get;
            private set;
        }

        public XmlMessage(XmlElement xml) : base(xml) {
            Body = null;
            if(!xml.HasChildNodes) throw new MessageParseException("Xml data has no child nodes to read.", xml);
            foreach(XmlNode node in xml.ChildNodes) if(node.Name == "body") Body = node;
            if(Body == null) throw new MessageParseException("No 'body' node to read.", xml);
            if(Body.Attributes["action"] == null) throw new MessageParseException("Xml message has no action.", xml);
            Action = Body.Attributes["action"].Value;
        }

        public XmlMessage(XmlElement xml, XmlNode body) : base(xml) {
            Body  = body;
            Action = Body.Attributes["action"].Value;
        }

        public static XmlNode Verify(XmlElement xml) {
            XmlNode body = null;
            if(!xml.HasChildNodes) throw new MessageParseException("Xml data has no child nodes to read.", xml);
            foreach(XmlNode node in xml.ChildNodes) if(node.Name == "body") body = node;
            if(body == null) throw new MessageParseException("No 'body' node to read.", xml);
            if(body.Attributes["action"] == null) throw new MessageParseException("Xml message has no action.", xml);
            return body;
        }
    }
}

