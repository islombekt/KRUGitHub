using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Users : IdentityUser
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public string SName { get; set; }
        public string Position { get; set; }
        [NotMapped]
        public string Role { get; set; }

        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        [JsonIgnore]
        public Department Department { get; set; }
        public int? AddressId { get; set; }
        [ForeignKey("AddressId")]
        [JsonIgnore]
        public Address Address { get; set; }
        public virtual Manager Managers { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
