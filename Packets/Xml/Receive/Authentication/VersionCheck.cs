/**
 * Received Version Check Packet for Snowbull's Plugin API ("Snowbull.API").
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

