using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KRU.Models
{
    public class Option
    {
        [Key]
        public int OptionId { get; set; }
        public string OptName { get; set; }
        public bool Correct { get; set; }
        public int? QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        public virtual ICollection<Empl_Selected_Option> Empl_Selected_Options { get; set; }
    }
}
