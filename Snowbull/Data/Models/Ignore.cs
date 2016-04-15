using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snowbull.Data.Models {
	[Table("ignore")]
	public class Ignore {
		[Key]
		[Column("ignoreID")]
		public int ID {
			get;
			set;
		}

		[Column("userID")]
		public int UserID {
			get;
			set;
		}

		[ForeignKey("UserID")]
		public User User {
			get;
			set;
		}

		[Column("ignoredID")]
		public int IgnoredID {
			get;
			set;
		}

		[ForeignKey("IgnoredID")]
		public User Ignored {
			get;
			set;
		}
	}
}

