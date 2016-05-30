/**
 * Received Login Packet for Snowbull's Plugin API ("Snowbull.API").
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of "Snowbull.API".
 * 
 * "Snowbull.API" is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * "Snowbull.API" is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with "Snowbull.API". If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

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

        public string Zone {
            get;
            private set;
        }

        public Login(XmlDocument document) : base(document) {
            XmlElement xml = document.DocumentElement;
            if(!Body.HasChildNodes) throw new MessageParseException("Message body has no child nodes to read.");
            XmlNode login = null;
            foreach(XmlNode node in Body.ChildNodes) if(node.Name == "login") login = node;
            if(login == null) throw new MessageParseException("Message body has no login node to read.");
            if(login.Attributes["z"] == null) throw new MessageParseException("Login has no zone attribute.");
            Zone = login.Attributes["z"].Value;
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

