using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Category1
    {
        [Key]
        public int Category1Id { get; set; }
        public string Name_C1 { get; set; }
        [NotMapped]
        public int FinCount { get; set; }
        [NotMapped]
        public int count11 { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public virtual ICollection<Category11> Category11s { get; set; }
        public virtual ICollection<FinanceReport> FinanceReports { get; set; }
    }
}
