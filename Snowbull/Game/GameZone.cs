﻿using System;
using Akka.Actor;

namespace Snowbull.Game {
    public class GameZone : Zone {
		private API.Plugin<API.Game.IGameZone>[] plugins;

		internal GameZone(IActorRef server, API.Plugins<API.IZone> basePlugins, API.Plugins<API.Game.IGameZone> plugins) : base(server, basePlugins) {
			this.plugins = plugins.Initialise();
        }

		protected override void Authenticate(Authenticate authenticate) {

        }
    }
}

