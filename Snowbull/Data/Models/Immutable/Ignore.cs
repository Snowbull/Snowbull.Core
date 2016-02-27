using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snowbull.Data.Models {
	public class Ignore {
		[ForeignKey("userID")]
		public User Ignorer {
			get;
			set;
		}

		[ForeignKey("ignoreID")]
		public User Ignored {
			get;
			set;
		}
	}
}

