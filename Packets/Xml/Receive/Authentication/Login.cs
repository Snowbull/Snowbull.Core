using System;
using System.Xml;

namespace Snowbull.API.Packets.Xml.Receive.Authentication {
    public class Login : XmlMessage, IReceivePacket {
        public string Username {
            get;
            private set;
        }

        public string Password {
            get;
            private set;
        }

        public Login(XmlDocument document) : base(document) {
            XmlElement xml = document.DocumentElement;
            if(!Body.HasChildNodes) throw new MessageParseException("Message body has no child nodes to read.");
            XmlNode login = null;
            foreach(XmlNode node in Body.ChildNodes) if(node.Name == "login") login = node;
            if(login == null) throw new MessageParseException("Message body has no login node to read.");
            if(!login.HasChildNodes) throw new MessageParseException("Login node has no nodes to read.");
            XmlNode nick = null;
            XmlNode pword = null;
            foreach(XmlNode node in login.ChildNodes)
                if(node.Name == "nick")
                    nick = node;
                else if(node.Name == "pword")
                    pword = node;
            if(nick == null) throw new MessageParseException("Login node has no nick node.");
            if(pword == null) throw new MessageParseException("Login node has no pword node.");
            Username = nick.InnerText;
            Password = pword.InnerText;
        }
    }
}

