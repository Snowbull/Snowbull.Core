/**
 * Configuration Section for Snowbull.
 *
 * Copyright 2016 by Lewis Hazell <staticabc@live.co.uk>
 *
 * This file is part of Snowbull.
 * 
 * Snowbull is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version
 * 3 of the License, or (at your option) any later version.
 * 
 * Snowbull is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty 
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See
 * the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Snowbull. If not, see <http://www.gnu.org/licenses/>.
 *
 * License: GPL-3.0 <https://www.gnu.org/licenses/gpl-3.0.txt>
 */

/**
 * Example
 *	<snowbull>
 *		<servers>
 *			<server id="1" name="Login" port="6112">
 *				<zones>
 *					<zone name="w1" type="Snowbull.Login.LoginZone" server="1" />
 *				</zones>
 *			</server>
 *			<server id="100" name="The Bull" port="9875">
 *				<zones>
 *					<zone name="w1" type="Snowbull.Game.GameZone" server="100" />
 *				</zones>
 *			</server>
 *		</servers>
 *	</snowbull>
 */

using System;
using System.Configuration;

namespace Snowbull.Configuration {
	public class SnowbullConfigurationSection : ConfigurationSection {
		public static SnowbullConfigurationSection GetConfiguration() {
			return (SnowbullConfigurationSection) ConfigurationManager.GetSection("snowbull") ?? new SnowbullConfigurationSection();
		}

		[ConfigurationProperty("servers")]
		[ConfigurationCollection(typeof(Servers), AddItemName = "server")]
		public Servers Servers {
			get {
				return this["servers"] as Servers;
			}
		}
	}
}

