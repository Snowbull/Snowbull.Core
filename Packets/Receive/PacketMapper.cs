using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Xml;

namespace Snowbull.API.Packets.Receive {
    public static class PacketMapper {
        public static ImmutableDictionary<string, Func<Xt.Parser, Xt.XtPacket>> XtMap() {
            Dictionary<string, Func<Xt.Parser, Xt.XtPacket>> map = new Dictionary<string, Func<Xt.Parser, Xt.XtPacket>>();
            return map.ToImmutableDictionary();
        }

        public static ImmutableDictionary<string, Func<XmlElement, Xml.XmlPacket>> XmlMap() {
            Dictionary<string, Func<XmlElement, Xml.XmlPacket>> map = new Dictionary<string, Func<XmlElement, Xml.XmlPacket>>();
            map.Add("rndK", xml => new Xml.Authentication.RandomKey(xml));
            map.Add("verChk", xml => new Xml.Authentication.VersionCheck(xml));
            map.Add("login", xml => new Xml.Authentication.Login(xml));
            return map.ToImmutableDictionary();
        }
    }
}

