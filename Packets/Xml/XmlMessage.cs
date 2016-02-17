using System;
using System.Xml;

namespace Snowbull.API.Packets.Xml {
    public class XmlMessage  : XmlPacket {
        public string Action {
            get;
            private set;
        }

        protected XmlNode Body {
            get;
            private set;
        }

        public XmlMessage(XmlDocument doc) : base(doc) {
            XmlElement xml = doc.DocumentElement;
            Body = null;
            if(!xml.HasChildNodes) throw new MessageParseException("Xml data has no child nodes to read.");
            foreach(XmlNode node in xml.ChildNodes) if(node.Name == "body") Body = node;
            if(Body == null) throw new MessageParseException("No 'body' node to read.");
            if(Body.Attributes["action"] == null) throw new MessageParseException("Xml message has no action.");
            Action = Body.Attributes["action"].Value;
        }

        public XmlMessage(XmlDocument doc, XmlNode body) : base(doc) {
            Body  = body;
            Action = Body.Attributes["action"].Value;
        }

        public XmlMessage(XmlDocument doc, string action, int r, XmlNode[] inner = null) : base(Create(doc, action, r, inner)) {

        }

        private static XmlDocument Create(XmlDocument document, string action, int r, XmlNode[] inner) {
            XmlElement msg = (XmlElement) document.AppendChild(document.CreateElement("msg"));
            msg.SetAttribute("t", "sys");
            XmlElement body = (XmlElement) msg.AppendChild(document.CreateElement("body"));
            body.SetAttribute("action", action);
            body.SetAttribute("r", r.ToString());
            if(inner != null)
                foreach(XmlNode node in inner) body.AppendChild(node);
            return document;
        }

        public static XmlNode Verify(XmlElement xml) {
            XmlNode body = null;
            if(!xml.HasChildNodes) throw new MessageParseException("Xml data has no child nodes to read.");
            foreach(XmlNode node in xml.ChildNodes) if(node.Name == "body") body = node;
            if(body == null) throw new MessageParseException("No 'body' node to read.");
            if(body.Attributes["action"] == null) throw new MessageParseException("Xml message has no action.");
            return body;
        }
    }
}

