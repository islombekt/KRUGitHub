using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace KRU.Models
{
    public class Test
    {
        [Key]
        public int TestId { get; set; }
        public string TestName { get; set; }
        public int RandomQuestions { get; set; }
        public string TestDescription { get; set; }
        public DateTime TestStarted { get; set; } 
        public DateTime TestEnd { get; set; }
        public int Time { get; set; }
        [NotMapped]
        public bool End { get; set; }
       

        [NotMapped]
        public int NumberOfUsers { get; set; }
        [NotMapped]
        public int NumberOfPassedUsers { get; set; }
        [NotMapped]
        public int ScoreOfUser { get; set; }
        [NotMapped]
        public int MinScoreOfUser { get; set; }
        public int? ManagerId { get; set; }
        [ForeignKey("ManagerId")]
        public virtual Manager Manager { get; set; }
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
        public virtual ICollection<Empl_Selected_Option> Empl_Selected_Options { get; set; }
        public virtual ICollection<Question> Questions { get; set; }

        public static implicit operator List<object>(Test v)
        {
            throw new NotImplementedException();
        }
    }
}
