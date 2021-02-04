using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public partial class Address
    {
        public Address()
        {
            Users = new HashSet<Users>();
        }

        [Key]
        public int AddressId { get; set; }
        [Required]
        public string Building { get; set; }
        
        public string City { get; set; } 
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<Objects> Objects { get; set; }
    }
}
