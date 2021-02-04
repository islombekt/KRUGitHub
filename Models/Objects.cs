using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Objects
    {
        [Key]
        public int ObjectId { get; set; }
        public string ObjectName { get; set; }
        
        public int? AddressId { get; set; }
        public virtual ICollection<Report> Reports { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
}
