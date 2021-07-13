using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Task_Report
    {
        [Key]
        public int Task_RepId { get; set; }
        public string CommentTR { get; set; }
        public int? TaskId { get; set; }
        public int? RepId { get; set; }
        [ForeignKey("TaskId")]
        public virtual Tasks Tasks { get; set; }
        [ForeignKey("RepId")]
        public virtual Report Report { get; set; }
    }
}
