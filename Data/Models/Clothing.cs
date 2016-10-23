using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Snowbull.Core.Data.Models {
    [Table("clothing")]
    public class Clothing {
        [Key]
        [Column("userID")]
        [ForeignKey("User")]
        public int UserID {
            get;
            set;
        }

        public User User {
            get;
            set;
        }

        [Column("head")]
        public int Head {
            get;
            set;
        }

        [Column("face")]
        public int Face {
            get;
            set;
        }

        [Column("neck")]
        public int Neck {
            get;
            set;
        }

        [Column("body")]
        public int Body {
            get;
            set;
        }

        [Column("hands")]
        public int Hands {
            get;
            set;
        }

        [Column("feet")]
        public int Feet {
            get;
            set;
        }

        [Column("colour")]
        public int Colour {
            get;
            set;
        }

        [Column("photo")]
        public int Photo {
            get;
            set;
        }

        [Column("pin")]
        public int Pin {
            get;
            set;
        }
    }
}

