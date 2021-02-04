using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Task_File
    {
        [Key]
        public int Task_FileId { get; set; }
        public int? TaskId { get; set; }
        public int? FileId { get; set; }
        [ForeignKey("TaskId")]
        public virtual Tasks Tasks { get; set; }
        [ForeignKey("FileId")]
        public virtual FileHistory FileHistory { get; set; }


    }
}
