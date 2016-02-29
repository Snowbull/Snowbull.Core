using System;
using System.Net;
using System.Collections.Generic;
using Akka.Actor;

namespace Snowbull {
    class Program {
        public static void Main(string[] args) {
			API.Assemblies assemblies = API.Assemblies.Load("Plugins/");
			API.Plugins<API.IZone> zonePlugins = assemblies.Get<API.IZone>();
			API.Plugins<API.Login.ILoginZone> loginZonePlugins = assemblies.Get<API.Login.ILoginZone>();
			API.Plugins<API.Game.IGameZone> gameZonePlugins = assemblies.Get<API.Game.IGameZone>();
            Dictionary<string, ZoneInitialiser> zones = new Dictionary<string, ZoneInitialiser>();
			zones.Add("w1", server => Login.LoginZone.Props("w1", server, zonePlugins, loginZonePlugins));
            Snowbull instance = new Snowbull(zones);
            instance.Bind(IPAddress.Loopback, 9000);
            Console.ReadLine();
        }
    }
}
