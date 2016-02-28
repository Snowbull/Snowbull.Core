using System;

namespace Snowbull.Data.Models.Immutable {
	public class ImmutableIgnore {
		public ImmutableUser Ignorer {
			get;
			private set;
		}

		public ImmutableUser Ignored {
			get;
			private set;
		}

		public ImmutableIgnore(ImmutableUser ignorer, ImmutableUser ignored) {
			Ignorer = ignorer;
			Ignored = ignored;
		}
	}
}

