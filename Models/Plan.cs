using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        public DateTime PlanStart { get; set; }
        public DateTime PlanEnd { get; set; }
        public string PlanDescription { get; set; }




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
