using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snowbull.Data.Models {
	[Table("furniture")]
	public class Furniture {
		[ForeignKey("userID")]
		public User Owner {
			get;
			set;
		}

		[Column("id")]
		public int Id {
			get;
			set;
		}
	}
}

