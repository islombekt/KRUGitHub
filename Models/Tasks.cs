using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        public string SumLost { get; set; }
        public string SumGain { get; set; }
        public string Comment { get; set; }
        [NotMapped]
        public List<int> selectedFiles { get; set; }
        public string File { get; set; }
        public bool Finished { get; set; } // finish the task
        public DateTime TaskStarted { get; set; } //manager
        public DateTime TaskEnd { get; set; } // manager
     
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public int? TaskTypeId { get; set; }
        [ForeignKey("TaskTypeId")]
        public virtual Task_Type Task_Type { get; set; }
        public virtual ICollection<Report> Reports { get; set; }
        public virtual ICollection<Task_File> Task_Files { get; set; }

    }
}
