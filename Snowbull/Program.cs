﻿using System;
using System.Net;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
            Dictionary<string, ZoneInitialiser> zones = new Dictionary<string, ZoneInitialiser>();
			zones.Add("w1", server => Login.LoginZoneActor.Props("w1", server));
            Snowbull instance = new Snowbull("Snowbull-Test", zones);
            instance.Bind(IPAddress.Loopback, 9000);
            Console.ReadLine();
        }
    }
}
