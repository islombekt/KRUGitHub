using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string EmployeeState { get; set; }
        public double Score { get; set; }
        public string FileUrl { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public int RepCount { get; set; } //also test id
        [NotMapped]
        public int? FinCount { get; set; }//also test score
        [NotMapped]
        public int PlanCount { get; set; }

        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Manager Manager { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]

        public virtual Users User { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<Empl_Selected_Option> Empl_Selected_Options { get; set; }
        public virtual ICollection<FinanceReport> FinanceReports { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
        public virtual ICollection<Task_Empl> Task_Emples { get; set; }
    }
}
