using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Category11
    {
        [Key]
        public int Category11Id { get; set; }
        public string Name_C11 { get; set; }
        [NotMapped]
        public int FinCount { get; set; }
        [NotMapped]
        public int Count111 { get; set; }
        public int? Category1Id { get; set; }
        [ForeignKey("Category1Id")]
        public virtual Category1 Category1 { get; set; }
        public virtual ICollection<Category111> Category111s { get; set; }
        public virtual ICollection<FinanceReport> FinanceReports { get; set; }
    }
}
