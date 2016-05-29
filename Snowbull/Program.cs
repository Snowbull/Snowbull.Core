using System;
using System.Net;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
            Dictionary<string, ZoneInitialiser> zones = new Dictionary<string, ZoneInitialiser>();
			zones.Add("w1", (server, oparent) => Login.LoginZoneActor.Props("w1", server, oparent));
            Snowbull instance = new Snowbull("Snowbull-Test", zones);
			instance.Bind(IPAddress.IPv6Any, 6112);
            Console.ReadLine();
        }
    }
}
