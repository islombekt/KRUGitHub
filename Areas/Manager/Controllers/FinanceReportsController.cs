using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;
using Microsoft.AspNetCore.Identity;
using ClosedXML.Excel;
using System.IO;

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class FinanceReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public FinanceReportsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

       
        // GET: Manager/FinanceReports
        public async Task<IActionResult> Index()
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var applicationDbContext = _context.FinanceReports.Include(f => f.Address).Include(f => f.Category1).Include(f => f.Category11)
                .Include(f => f.Category111).Include(f => f.Employee).ThenInclude(u => u.User).Include(f => f.Manager).ThenInclude(u => u.User)
                .Include(f => f.Objects).Include(f => f.Tasks).Where(r => r.ManagerId == ManId).OrderByDescending(u => u.FinanceRepId);
            return View(await applicationDbContext.ToListAsync());
        }


        [HttpPost]
        public IActionResult SpravkaExcel(DateTime start, DateTime end)
        {

            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var dep = _context.Managers.Include(u => u.User).ThenInclude(d => d.Department).FirstOrDefault(i => i.ManagerId == ManId);
            var Employees = _context.Employees.Include(u => u.User).ThenInclude(d => d.Department).ToList().Where(i => i.ManagerId == ManId);
            var Reportss = _context.Reports.ToList().Where(i => i.ReportDate.Date >= start.Date && i.ReportDate.Date <= end.Date && i.ManagerId == ManId);
            var FinanceReports = _context.FinanceReports.Include(a => a.Address).Include(e => e.Employee).ThenInclude(t => t.User).Include(t =>t.Tasks).ThenInclude(a => a.Task_Files).ThenInclude(i =>i.FileHistory)
                .Include(i => i.Category1).Include(i => i.Category11).Include(i => i.Category111).ToList()
                .Where(i => i.EndDate.Date >= start.Date && i.EndDate.Date <= end.Date && i.ManagerId == ManId);
            var taskList = FinanceReports.Select(i => i.TaskId).Distinct();
            var EmplList = FinanceReports.Select(i => i.EmployeeId).Distinct();
            var AddressList = FinanceReports.Select(i => i.AddressId).Distinct();
            var cat1Get = new List<Tuple<int, int>>();
            
        
            using (var workbook = new XLWorkbook())
            {

                #region
                var jadval = workbook.Worksheets.Add("Хисобот жадвали");
                int af = 3;
                
                jadval.Row(6).Style.NumberFormat.SetFormat("#,##0.00");
                var tashkilot = jadval.Range("A4:A5");
                tashkilot.Value = "Ташкилот номи";
                tashkilot.Merge();
                tashkilot.Style.Alignment.WrapText = true;
                tashkilot.Style.Font.Bold = true;
                jadval.Column("A").Width = 40;
                jadval.Cell("A6").Value = "";
                var tekshiruvDavri = jadval.Range("B4:B5");
                tekshiruvDavri.Value = "Ўтказилган текшириш даври";
                tekshiruvDavri.Merge();
                tekshiruvDavri.Style.Alignment.WrapText = true;
                tekshiruvDavri.Style.Font.Bold = true;
                jadval.Column("B").Width = 25;
                jadval.Cell("A6").Style.Alignment.WrapText = true;
                int c = 0;
                int cc = 0;
                int plus = 0;
                int plus1 = 0;
              
               
                    foreach (var cy1 in _context.Category1s.ToList().OrderBy(i => i.Category1Id))
                    {
                        var cccc = jadval.Range(4, c + 3, 5, c + 3);
                        cccc.Value = cy1.Name_C1;
                        cccc.Merge();
                        cccc.Style.Alignment.WrapText = true;
                        cccc.Style.Font.Bold = true;
                        jadval.Column(c + 3).Width = 25;
                        
                        var temp = _context.Category11s.ToList().Where(i => i.Category1Id == cy1.Category1Id);
                       // jadval.Cell(6, c + 3).FormulaR1C1 = "=spravka!R" + fa + "C7";
                        c++;

                        if (temp != null && temp.Any())
                        {
                        cat1Get.Add(new Tuple<int, int>(cy1.Category1Id, c +2));
                        plus1 = plus;
                            cc = c + 3;
                            foreach (var cy2 in _context.Category11s.ToList().OrderBy(i => i.Category11Id).Where(i => i.Category1Id == cy1.Category1Id))
                            {

                                jadval.Cell(5, c + 3).Value = cy2.Name_C11;
                                jadval.Cell(5, c + 3).Style.Alignment.WrapText = true;
                                jadval.Cell(5, c + 3).Style.Font.Bold = true;
                                jadval.Column(c + 3).Width = 25;
                               
                                var temp1 = _context.Category111s.ToList().Where(i => i.Category11Id == cy2.Category11Id);
                            //    jadval.Cell(6, c + 3).FormulaR1C1 = "=spravka!R" + fa1 + "C7";
                                plus1 += temp1.Count();
                                //if (temp1.Any())
                                //{
                                //    check = 1;
                                //}
                                c++;


                            }

                            var shundan = jadval.Range(4, cc, 4, c + 2);
                            shundan.Value = "Шундан:";
                            shundan.Merge();
                            shundan.Style.Font.Bold = true;
                        }
                      
                    plus = plus1;



                    }
                //foreach (int EId in EmplList)
                // {
                int addRow = 6;
                foreach (int i in AddressList)
                {
                    jadval.Cell(addRow, 1).Value = _context.Addresses.FirstOrDefault(t => t.AddressId == i).Building;
                    var check = FinanceReports.FirstOrDefault(tm => tm.AddressId == i && tm.EmployeeId == tm.Tasks.MasulEmplId && tm.CheckPeriod != null);
                    if(check != null)
                    {
                        jadval.Cell(addRow, 2).Value = check.CheckPeriod;
                    }    
                   
                        foreach (var fin in FinanceReports.Where(tm => tm.AddressId == i  && tm.EmployeeId == tm.Tasks.MasulEmplId))
                        {
                            for (int column = 3; column <= c + 2; column++)
                            {
                            var f11 = _context.Category11s.ToList().FirstOrDefault(a => a.Category11Id == fin.Category11Id);
                            var f1 = _context.Category1s.ToList().FirstOrDefault(a => a.Category1Id == fin.Category1Id);
                            string f = jadval.Cell(5, column).Value.ToString();
                            //var ff = jadval.Range(4, column,5,column);
                            //ff.Merge();
                            
                            string fs = jadval.Cell(4, column).Value.ToString();
                            if (fin.Category11Id == null && fin.Category1Id != null)
                            {
                                if (f1 != null && f1.Name_C1 == fs)
                                {
                                    _ = Int32.TryParse(jadval.Cell(addRow, column).Value.ToString(), out int amountaa);


                                    jadval.Cell(addRow, column).Value = amountaa +  fin.amount ;

                                }
                               
                            }
                            if (f11 != null && f11.Name_C11 == f)
                            {
                                jadval.Cell(addRow, column).Value = fin.amount;

                            }
                               
                            }
                              
                                af++;
                        }
                    foreach(var id in cat1Get)
                    {
                        int counsdt = _context.Category11s.ToList().Where(i => i.Category1Id == id.Item1).Count(); // not endeddddddddd use count for formula range
                        
                        jadval.Cell(addRow, id.Item2).SetFormulaR1C1("=SUM(R" + addRow +"C"+(id.Item2+1) + ":R" +addRow+"C"+(id.Item2+counsdt)+")");
                    }   
                    addRow++;
                    //}


                    //}

                }
                var main = jadval.Range(1, 1, 1, c + 2);
                main.Merge();
                main.Value = "Молиявия ва комплаенс назорати департаменти томонидан \"Ўзбекнефтгаз\" АЖ тизимидаги корхона " +
                    "ва ташкилотларда "+end.Year+" йил якунига қадар ўтказилган ўрганишлар таҳлиллар натижасида аниқланган молиявий қонун бузилиши ҳолатлари ва амалга оширилган ишлар бўйича тезкор";
                main.Style.Font.Bold = true;
                var main2 = jadval.Range(2, 1, 2, c + 2);
                main2.Merge();
                main2.Value = "М А Ъ Л У М О Т ";
                main2.Style.Font.Bold = true;
                jadval.Rows(6, addRow).Style.NumberFormat.SetFormat("#,##0.00");
                jadval.Rows(6,addRow-1).Height = 50;
                jadval.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                jadval.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                jadval.SetShowGridLines(true);
                var Bbjad = jadval.Range(4, 1, addRow, c + 3);
                Bbjad.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                Bbjad.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                jadval.Row(addRow).Style.Fill.SetBackgroundColor(XLColor.PastelGray);
                for (int i = 3; i < c+3; i++)
                {
                    jadval.Cell(addRow, i).SetFormulaR1C1("=SUM(R" + 6 + "C" + i + ":R" + (addRow-1) + "C" + i + ")");
                }
                #endregion
                var sheet4 = workbook.Worksheets.Add("Хисобот");
                var hisobot1 = sheet4.Range("J1:L1");
                hisobot1.Value = "“Ўзбекнефтгаз” АЖ";
                hisobot1.Merge();

                sheet4.Rows(1, 5).Style.Font.Bold = true;

                var hisobot2 = sheet4.Range("J2:L2");
                hisobot2.Value = "Бошқарув раиси";
                hisobot2.Merge();

                var hisobot3 = sheet4.Range("J3:L3");
                hisobot3.Value = "М.Р. Абдуллаевга";
                hisobot3.Merge();

               
                var hisobot4 = sheet4.Range("A5:L5");
                hisobot4.Value = "Ҳурматли Мехриддин Раззоқович!";
                hisobot4.Merge();
                hisobot4.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                var hisobot5 = sheet4.Range("A6:L6");
                hisobot5.Value = "Молиявий ва комплаенс назорати департаменти томонидан жорий йилнинг  "+start.ToShortDateString() +" йилдан  "+ end.ToShortDateString() + " йилгача ";
                hisobot5.Merge();
                var hisobot512 = sheet4.Range("A7:F7");
                hisobot512.Value = "бўлган даврида қуйидаги ишлар амалга оширилди.";
                hisobot512.Merge();
                var hisobot6 = sheet4.Range("B8:K8");
                hisobot6.Value = "Молиявий назорат йўналишида ташкилотлар бўйича амалга оширилган ўрганиш ва таҳлиллар:";
                hisobot6.Merge();
                hisobot6.Style.Font.Bold = true;
                int ffrow = 9;
                int cold = 3;
                foreach (var cccat1 in _context.Category1s.ToList())
                {
                   
                    var hisobot7 = sheet4.Range("B"+ffrow+":H"+ffrow);
                    hisobot7.Value = "Ўтказилган ўрганишларда " +cccat1.Name_C1;
                    hisobot7.Merge();
                    sheet4.Cell("I" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R"+addRow+"C"+cold);
                    var hisobot71 = sheet4.Range("I" + ffrow + ":J" + ffrow);
                    hisobot71.Merge();
                    sheet4.Cell("K" + ffrow).Value = "cўмни ташкил этади.";
                    if(_context.Category11s.ToList().Where(i => i.Category1Id == cccat1.Category1Id).Count() > 0)
                    {
                        sheet4.Cell("L" + ffrow).Value = "Шундан";
                    }
                   
                    cold++;
                    ffrow++;
                    int id = 1;
                    foreach (var cccat2 in _context.Category11s.ToList().Where(i => i.Category1Id == cccat1.Category1Id))
                    {
                        if(cccat2 != null)
                        {
                           
                            if (id % 2 == 0)
                            {
                                //sheet4.Cell("E" + (ffrow-1)).SetFormulaR1C1("=\'Хисобот жадвали\'!R5C4");
                                sheet4.Cell("G" + (ffrow)).Value = cccat2.Name_C11;
                                var hisobot82 = sheet4.Range("G" + (ffrow) + ":I" + (ffrow));
                                hisobot82.Merge();
                                sheet4.Cell("J" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R" + addRow + "C" + cold);
                                var hisobot83 = sheet4.Range("J" + (ffrow) + ":K" + (ffrow));
                                hisobot83.Merge();
                                sheet4.Cell("L" + ffrow).Value = "cўмни ташкил этади.";
                                sheet4.Cell("F" + ffrow).Value = " ва ";
                                cold++;
                                id++;
                                ffrow++;
                            }
                            else
                            {
                                sheet4.Cell("A" + ffrow).Value = "Умумий ";
                                //sheet4.Cell("B" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R5C4");
                                sheet4.Cell("B" + (ffrow)).Value = cccat2.Name_C11;
                                var hisobot81 = sheet4.Range("B" + ffrow + ":D" + ffrow);
                                hisobot81.Merge();
                                sheet4.Cell("E" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R" + addRow + "C" + cold);
                                cold++;
                                id++;
                            }
                          
                        }
                        if(_context.Category11s.ToList().Where(i => i.Category1Id == cccat1.Category1Id).Count()%2 == 1)
                        {
                            ffrow++;
                        }
                       
                        

                    }
                }

                cold = 3;
                int ids = 1;
                int addressCountRow = 6;

                foreach (int i in AddressList)
                {
                    
                    var s = _context.Addresses.FirstOrDefault(tm => tm.AddressId == i);
                    var asos = FinanceReports.FirstOrDefault(t => t.AddressId == i);
                    var asId = asos.Tasks.Task_Files.Select(i => i.FileId);
                    string Asos1 = "";
                    foreach(int id in asId)
                    {
                        Asos1 +=" " + _context.FileHistory.FirstOrDefault(i => i.FileId == id).Name; 
                    }
                    if (s != null)
                    {
                        var Add1 = sheet4.Range("B" + ffrow + ":K" + ffrow);
                        Add1.Value = ids+"." + Asos1;
                        Add1.Merge();
                        ffrow++;
                        var Add2 = sheet4.Range("A" + ffrow + ":B" + ffrow);
                        Add2.Value = s.Building;
                        Add2.Merge();
                        var Add3 = sheet4.Range("C" + ffrow + ":L" + ffrow);
                        Add3.Value = "нинг фаолияти самарадорлигини ошириш, тўлов интизомини мустаҳкамлаш,  ";
                        Add3.Merge();
                        ffrow++;
                        var Add4 = sheet4.Range("A" + ffrow + ":L" + ffrow);
                        Add4.Value = "дебитор ва кредитор қарзларни камайтириш ҳамда  тизимда турли талон тарожликларни олдини олиш масалаларида  амалий ёрдам кўрсатиш";
                        Add4.Merge();
                        ffrow++;
                        var Add5 = sheet4.Range("A" + ffrow + ":D" + ffrow);
                        Add5.Value = "юзасидан ўрганиш ўтказилди.";
                        Add5.Merge();
                        ffrow++;
                    }
                    ids++;
                    foreach (var cccat1 in _context.Category1s.ToList())
                    {

                        var hisobot7 = sheet4.Range("B" + ffrow + ":H" + ffrow);
                        hisobot7.Value = "Ўтказилган ўрганишларда " + cccat1.Name_C1;
                        hisobot7.Merge();
                        sheet4.Cell("I" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R" + addressCountRow + "C" + cold);
                        var hisobot71 = sheet4.Range("I" + ffrow + ":J" + ffrow);
                        hisobot71.Merge();
                        sheet4.Cell("K" + ffrow).Value = "cўмни ташкил этади.";
                        if (_context.Category11s.ToList().Where(i => i.Category1Id == cccat1.Category1Id).Count() > 0)
                        {
                            sheet4.Cell("L" + ffrow).Value = "Шундан";
                        }

                        cold++;
                        ffrow++;
                        int id1 = 1;
                        foreach (var cccat2 in _context.Category11s.ToList().Where(i => i.Category1Id == cccat1.Category1Id))
                        {
                            if (cccat2 != null)
                            {

                                if (id1 % 2 == 0)
                                {
                                    //sheet4.Cell("E" + (ffrow-1)).SetFormulaR1C1("=\'Хисобот жадвали\'!R5C4");
                                    sheet4.Cell("G" + (ffrow)).Value = cccat2.Name_C11;
                                    var hisobot82 = sheet4.Range("G" + (ffrow) + ":I" + (ffrow));
                                    hisobot82.Merge();
                                    sheet4.Cell("J" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R" + addressCountRow + "C" + cold);
                                    var hisobot83 = sheet4.Range("J" + (ffrow) + ":K" + (ffrow));
                                    hisobot83.Merge();
                                    sheet4.Cell("L" + ffrow).Value = "cўмни ташкил этади.";
                                    sheet4.Cell("F" + ffrow).Value = " ва ";
                                    cold++;
                                    id1++;
                                    ffrow++;
                                }
                                else
                                {
                                    sheet4.Cell("A" + ffrow).Value = "Умумий ";
                                    //sheet4.Cell("B" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R5C4");
                                    sheet4.Cell("B" + (ffrow)).Value = cccat2.Name_C11;
                                    var hisobot81 = sheet4.Range("B" + ffrow + ":D" + ffrow);
                                    hisobot81.Merge();
                                    sheet4.Cell("E" + ffrow).SetFormulaR1C1("=\'Хисобот жадвали\'!R" + addressCountRow + "C" + cold);
                                    cold++;
                                    id1++;
                                }

                            }
                            if (_context.Category11s.ToList().Where(i => i.Category1Id == cccat1.Category1Id).Count() % 2 == 1)
                            {
                                ffrow++;
                            }



                        }
                    }
                    addressCountRow++;
                    cold = 3;
                }

                    sheet4.Rows(6, ffrow).Style.NumberFormat.SetFormat("#,##0.00");
             
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Spravka.xlsx"
                        );
                }

            }

        }


        // GET: Manager/FinanceReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financeReport = await _context.FinanceReports
                .Include(f => f.Address)
                .Include(f => f.Category1)
                .Include(f => f.Category11)
                .Include(f => f.Category111)
                .Include(f => f.Employee).ThenInclude(u => u.User)
                .Include(f => f.Manager).ThenInclude(u => u.User)
                .Include(f => f.Objects)
                .Include(f => f.Tasks)
                .FirstOrDefaultAsync(m => m.FinanceRepId == id);
            financeReport.HaveSeen = true;
            try
            {
                _context.Update(financeReport);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinanceReportExists(financeReport.FinanceRepId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (financeReport == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", financeReport);
        }

        // GET: Manager/FinanceReports/Create
        public IActionResult Create()
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11");
            ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111");
           ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectName");
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                   
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View();
        }

        [HttpPost]
        public IActionResult CreateAfterCat(int FinanceRepId, DateTime StartDate, string Status,DateTime EndDate, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, bool HaveSeen, string CheckPeriod)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");



            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            //ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111");

            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                CheckPeriod = CheckPeriod,
                Status = Status
                
            };
            return View("Create", finance);
        }
        [HttpPost]
        public IActionResult CreateAfterCat1(int FinanceRepId, DateTime StartDate, DateTime EndDate, bool HaveSeen, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string Status, string CheckPeriod)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                CheckPeriod = CheckPeriod,
                Status = Status
            };
            return View("Create", finance);
        }

        [HttpPost]
        public IActionResult CreateAfterAddress(int FinanceRepId, DateTime StartDate, bool HaveSeen, DateTime EndDate, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod, string Status)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");

            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.ObjectName,
                    Value = item.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                items.Add(s);
            }
            ViewData["ObjectId"] = items;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                CheckPeriod = CheckPeriod,
                Status = Status
            };
            return View("Create", finance);
        }

        // POST: Manager/FinanceReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FinanceRepId,StartDate,EndDate,amount,FinComment,TaskId,Category1Id,Category11Id,Category111Id,AddressId,ObjectId,ManagerId,EmployeeId, HaveSeen,CheckPeriod, Status")] FinanceReport financeReport)
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            financeReport.ManagerId = ManId;
            financeReport.HaveSeen = false;
            //financeReport.amount = 12.501;
          //  financeReport.amount =Convert.ToDouble(financeReport.amount);
            if (ModelState.IsValid)
            {
               // financeReport.amount = Convert.ToDouble(financeReport.amount.ToString().Replace(",", "."));
                _context.Add(financeReport);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", financeReport.AddressId);
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", financeReport.Category1Id);
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", financeReport.Category11Id);
            ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111", financeReport.Category111Id);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", financeReport.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectName", financeReport.ObjectId);
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View(financeReport);
        }

        // GET: Manager/FinanceReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financeReport = await _context.FinanceReports.FindAsync(id);
            financeReport.HaveSeen = true;
            try
            {
                _context.Update(financeReport);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FinanceReportExists(financeReport.FinanceRepId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (financeReport == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", financeReport.AddressId);
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", financeReport.Category1Id);
            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == financeReport.Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == financeReport.Category11Id && i.Category11.Category1Id == financeReport.Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;

            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", financeReport.ManagerId);

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == financeReport.AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.ObjectName,
                    Value = item.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                items.Add(s);
            }
            ViewData["ObjectId"] = items;

            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View(financeReport);
        }

        [HttpPost]
        public IActionResult EditAfterCat(int FinanceRepId, DateTime StartDate, DateTime EndDate, int ManagerId, int EmployeeId, bool HaveSeen, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod, string Status)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");

            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");


            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            //ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111");

            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                ManagerId = ManagerId,
                EmployeeId = EmployeeId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen, Status = Status,
                CheckPeriod= CheckPeriod
            };
            return View("Edit", finance);
        }
        [HttpPost]
        public IActionResult EditAfterCat1(int FinanceRepId, DateTime StartDate, DateTime EndDate, bool HaveSeen,int ManagerId, int EmployeeId, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod, string Status)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                ManagerId = ManagerId,
                EmployeeId = EmployeeId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                Status = Status,
                CheckPeriod = CheckPeriod
            };
            return View("Edit", finance);
        }

        [HttpPost]
        public IActionResult EditAfterAddress(int FinanceRepId, DateTime StartDate, DateTime EndDate, bool HaveSeen, double amount, int ManagerId, int EmployeeId, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod, string Status)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");


            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                ManagerId = ManagerId,
                EmployeeId =EmployeeId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                Status = Status,
                CheckPeriod = CheckPeriod
            };
            return View("Edit", finance);
        }


        // POST: Manager/FinanceReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int FinanceRepId, [Bind("FinanceRepId,StartDate,EndDate,amount,FinComment,TaskId,Category1Id,Category11Id,Category111Id,AddressId,ObjectId,ManagerId,EmployeeId, HaveSeen,CheckPeriod, Status")] FinanceReport financeReport)
        {
            if (FinanceRepId != financeReport.FinanceRepId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(financeReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinanceReportExists(financeReport.FinanceRepId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", financeReport.AddressId);
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", financeReport.Category1Id);
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", financeReport.Category11Id);
            ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111", financeReport.Category111Id);
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", financeReport.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectName", financeReport.ObjectId);
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View(financeReport);
        }

        // GET: Manager/FinanceReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financeReport = await _context.FinanceReports
                .Include(f => f.Address)
                .Include(f => f.Category1)
                .Include(f => f.Category11)
                .Include(f => f.Category111)
                .Include(f => f.Employee).ThenInclude(u => u.User)
                .Include(f => f.Manager).ThenInclude(u => u.User)
                .Include(f => f.Objects)
                .Include(f => f.Tasks)
                .FirstOrDefaultAsync(m => m.FinanceRepId == id);
            if (financeReport == null)
            {
                return NotFound();
            }

            return View(financeReport);
        }

        // POST: Manager/FinanceReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var financeReport = await _context.FinanceReports.FindAsync(id);
            _context.FinanceReports.Remove(financeReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinanceReportExists(int id)
        {
            return _context.FinanceReports.Any(e => e.FinanceRepId == id);
        }
    }
}
