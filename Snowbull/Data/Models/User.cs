using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace Snowbull.Data.Models {
    [Table("users")]
    public class User {
        [Key]
        [Column("userID")]
        public int Id {
            get;
            set;
        }

        [Column("username")]
        [MaxLength(12)]
        public string Username {
            get;
            set;
        }

        [Column("password")]
        [MaxLength(64)]
        public string Password {
            get;
            set;
        }

        [Column("creation")]
        public DateTime Creation {
            get;
            set;
        }

        [Column("played")]
        public TimeSpan Played {
            get;
            set;
        }

		public virtual ICollection<Item> Inventory {
			get;
			set;
		}

		public virtual ICollection<Ignore> Ignores {
			get;
			set;
		}
    }
}

