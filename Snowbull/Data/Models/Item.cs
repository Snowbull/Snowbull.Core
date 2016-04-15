using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snowbull.Data.Models {
	[Table("inventory")]
	public class Item {
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

		[Column("item")]
		public int Id {
			get;
			set;
		}
	}
}

