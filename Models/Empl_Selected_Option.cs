using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KRU.Models
{
    public class Empl_Selected_Option
    {
        [Key]
        public int SelectedId { get; set; }
        public int? EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }
        public int? TestId { get; set; }
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        public int? QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
        public int? OptionId { get; set; }
        [ForeignKey("OptionId")]
        public virtual Option Option { get; set; }
    }
}
