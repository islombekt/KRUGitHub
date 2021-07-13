using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KRU.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [NotMapped]
        public int NumberOfOptions { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string CorrectOption { get; set; }
        [NotMapped]
        public string UserSelectedOption { get; set; }
        [NotMapped]
        public bool IsUserSelectedOptionCorrect { get; set; }
        public string QName { get; set; }
        public int QWeight { get; set; }
        public int? TestId { get; set; }
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
        public virtual ICollection<Empl_Selected_Option> Empl_Selected_Options { get; set; }
        public virtual ICollection<Option> Options { get; set; }
    }
}
