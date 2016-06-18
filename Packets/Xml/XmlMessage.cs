/**
 * Base XML Message for Snowbull's Plugin API ("Snowbull").
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
using System.Xml;

namespace Snowbull.Packets.Xml {
    public abstract class XmlMessage  : XmlPacket {
        public string Action {
            get;
            private set;
        }

        protected XmlNode Body {
            get;
            private set;
        }

        internal XmlMessage(XmlDocument doc) : base(doc) {
            XmlElement xml = doc.DocumentElement;
            Body = null;
            if(!xml.HasChildNodes) throw new MessageParseException("Xml data has no child nodes to read.");
            foreach(XmlNode node in xml.ChildNodes) if(node.Name == "body") Body = node;
            if(Body == null) throw new MessageParseException("No 'body' node to read.");
            if(Body.Attributes["action"] == null) throw new MessageParseException("Xml message has no action.");
            Action = Body.Attributes["action"].Value;
        }

        internal XmlMessage(XmlDocument doc, XmlNode body) : base(doc) {
            Body  = body;
            Action = Body.Attributes["action"].Value;
        }

        internal XmlMessage(XmlDocument doc, string action, int r, XmlNode[] inner = null) : base(Create(doc, action, r, inner)) {

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

