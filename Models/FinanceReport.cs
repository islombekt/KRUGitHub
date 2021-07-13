using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class FinanceReport
    {
        [Key]
        public int FinanceRepId { get; set; }
        public DateTime StartDate { get; set; }
       
        public DateTime EndDate { get; set; }
        public bool HaveSeen { get; set; }
        public string CheckPeriod { get; set; }
        public double amount { get; set; }
        public string Status { get; set; }
        public string FinComment { get; set; }
        [NotMapped]
        public int FinCount { get; set; }
        public int? TaskId { get; set; }
        public int? Category1Id { get; set; }
        public int? Category11Id { get; set; }
        public int? Category111Id { get; set; }
        public int? AddressId { get; set; }
        public int? ObjectId { get; set; }
        public int? ManagerId { get; set; }
        public int? EmployeeId { get; set; }
        [ForeignKey("Category111Id")]
        public virtual Category111 Category111 { get; set; }
        [ForeignKey("Category11Id")]
        public virtual Category11 Category11 { get; set; }
        [ForeignKey("Category1Id")]
        public virtual Category1 Category1{ get; set; }

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
