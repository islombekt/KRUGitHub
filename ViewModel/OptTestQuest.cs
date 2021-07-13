using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KRU.Models;
namespace KRU.ViewModel
{
    public class OptTestQuest
    {
        public IEnumerable<Test> Tesst { get; set; }
        public IEnumerable<Question> Questionss { get; set; }
        public IEnumerable<Option> Optionss { get; set; }
        public IEnumerable<Empl_Selected_Option> EmpOptionss { get; set; }

    }
}
