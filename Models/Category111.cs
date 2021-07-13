using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Category111
    {
        [Key]
        public int Category111Id { get; set; }
        public string Name_C111 { get; set; }

        [NotMapped]
        public int FinCount { get; set; }
        public int? Category11Id { get; set; }
        [ForeignKey("Category11Id")]
        public virtual Category11 Category11 { get; set; }
        public virtual ICollection<FinanceReport> FinanceReports { get; set; }
    }
}
