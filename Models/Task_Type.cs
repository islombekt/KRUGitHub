using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.Models
{
    public class Task_Type
    {
        [Key]
        public int TaskTypeID { get; set; }
        public string NameType { get; set; }
        public virtual ICollection<Tasks> Tasks { get; set; }
        public virtual ICollection<FileHistory> FileHistory { get; set; }
    }
}
