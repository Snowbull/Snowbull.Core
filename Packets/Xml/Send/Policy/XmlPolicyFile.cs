/**
 * XML Policy Packet for Snowbull's Plugin API ("Snowbull").
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
 
using System.Xml;
using System.Collections.Immutable;

namespace Snowbull.Core.Packets.Xml.Send.Policy {
	public sealed class XmlPolicyFile : XmlPacket, ISendPacket {
		public ImmutableArray<Rule> Rules {
			get;
			private set;
		}

		private XmlPolicyFile(XmlDocument document, Rule[] rules) : base(document) {
			Rules = rules.ToImmutableArray();
		}

		public static XmlPolicyFile Create(Rule[] rules) {
			XmlDocument document = new XmlDocument();
			XmlElement policy = (XmlElement) document.AppendChild(document.CreateElement("cross-domain-policy"));
			foreach(Rule rule in rules) {
				XmlElement child = (XmlElement) policy.AppendChild(document.CreateElement(rule.Tag));
				child.SetAttribute("domain", rule.Host);
				child.SetAttribute("to-ports", rule.Port);
			}
			return new XmlPolicyFile(document, rules);
		}
	}
}

