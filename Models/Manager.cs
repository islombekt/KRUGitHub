using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Manager
    {
        [Key]
        public int ManagerId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public virtual Users User { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<FinanceReport> FinanceReports { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
    }
}
