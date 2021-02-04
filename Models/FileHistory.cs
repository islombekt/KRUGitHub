using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class FileHistory
    {
        [Key]
        public int FileId { get; set; }
        public string Name { get; set; }
        public string FileUrl { get; set; }
        public string Description { get; set; }
        public DateTime FileStart { get; set; }
        public DateTime FileEnd { get; set; }
        public bool FileFinished { get; set; } // auto mark when every department will close the tasks

        public int? TaskTypeId { get; set; }
        [ForeignKey("TaskTypeId")]
        public virtual Task_Type Task_Type { get; set; }
        public virtual ICollection<Task_File> Task_Files { get; set; }
    }
}
