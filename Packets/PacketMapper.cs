/**
 * Packet to Class Map for Snowbull's Plugin API ("Snowbull").
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
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Xml;

namespace Snowbull.Packets {
    public static class PacketMapper {
        public static ImmutableDictionary<string, Func<Xt.XtData, Xt.XtPacket>> XtMap() {
            Dictionary<string, Func<Xt.XtData, Xt.XtPacket>> map = new Dictionary<string, Func<Xt.XtData, Xt.XtPacket>>();
            map.Add("j#js", xt => new Xt.Receive.Authentication.JoinServer(xt));
            return map.ToImmutableDictionary();
        }

        public static ImmutableDictionary<string, Func<XmlDocument, Xml.XmlPacket>> XmlMap() {
            Dictionary<string, Func<XmlDocument, Xml.XmlPacket>> map = new Dictionary<string, Func<XmlDocument, Xml.XmlPacket>>();
            map.Add("rndK", xml => new Xml.Receive.Authentication.RandomKey(xml));
            map.Add("verChk", xml => new Xml.Receive.Authentication.VersionCheck(xml));
            map.Add("login", xml => new Xml.Receive.Authentication.Login(xml));
            return map.ToImmutableDictionary();
        }
    }
}

