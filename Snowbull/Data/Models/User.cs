using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Username {
            get;
            set;
        }

        [Column("password")]
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
    }
}

