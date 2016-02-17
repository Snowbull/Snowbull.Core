using System;
using System.Xml;
namespace Snowbull.API.Packets.Xml.Receive.Authentication {
    public class RandomKey : XmlMessage, IReceivePacket {
        public RandomKey(XmlDocument xml) : base(xml) {
            
        }
    }
}

