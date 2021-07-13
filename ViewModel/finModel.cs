using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KRU.ViewModel
{
    public class finModel
    {
        public int AddressId { get; set; }
        public string CheckPeriod { get; set; }
        public string __RequestVerificationToken { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Status { get; set; }
        public int TaskId { get; set; }
        public List<CatModel> category { get; set; }
        public finModel()
        {
            category = new List<CatModel>();
        }
    }
}
