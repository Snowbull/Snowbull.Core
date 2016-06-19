using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snowbull.Core.Data.Models {
    [Table("users")]
    public class Credentials {
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
    }
}

