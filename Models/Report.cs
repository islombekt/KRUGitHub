using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        public string State { get; set; } //Worker State
        public DateTime ReportDate { get; set; }
        public string ReportDescription { get; set; }
        public string ReportComment { get; set; } // from manager side
        public double ReportScore { get; set; }
        
        
        public int? TaskId { get; set; }
        public int? AddressId { get; set; }
        public int? ObjectId { get; set; }
        public int? ManagerId { get; set; }
        public int? EmployeeId { get; set; }

        [ForeignKey("ObjectId")]
        public virtual Objects Objects { get; set; }
        [ForeignKey("TaskId")]
        public virtual Tasks Tasks { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Manager Manager { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
    }
}
