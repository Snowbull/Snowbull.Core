using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snowbull.Data.Models {
	[Table("furniture")]
	public class Furniture {
		[Column("userID")]
		public int OwnerID {
			get;
			set;
		}

		[ForeignKey("OwnerID")]
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

