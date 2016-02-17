using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Xml;

namespace Snowbull.API.Packets {
    public static class PacketMapper {
        public static ImmutableDictionary<string, Func<Xt.XtData, Xt.XtPacket>> XtMap() {
            Dictionary<string, Func<Xt.XtData, Xt.XtPacket>> map = new Dictionary<string, Func<Xt.XtData, Xt.XtPacket>>();
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

