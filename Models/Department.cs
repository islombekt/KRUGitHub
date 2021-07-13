using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public virtual ICollection<Users> Users { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public virtual ICollection<Category1> Category1s { get; set; }
    }
}
