using System;
using System.Xml;
using System.Collections.Immutable;

namespace Snowbull.API.Packets.Xml.Send.Policy {
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

