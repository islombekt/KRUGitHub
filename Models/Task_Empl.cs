using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Task_Empl
    {
        [Key]
        public int Task_EmpId { get; set; }
    
        public int? TaskId { get; set; }
        public int? EmplId { get; set; }
        [ForeignKey("TaskId")]
        public virtual Tasks Tasks { get; set; }
        [ForeignKey("EmplId")]
        public virtual Employee Employee { get; set; }
    }
}
