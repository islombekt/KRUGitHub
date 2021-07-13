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
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Util;
using NPOI.SS.UserModel;
using ClosedXML.Excel;
using System.IO;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Authorization;
//using GenerateAndDownloadExcel.Models;

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _iweb;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ReportsController(IWebHostEnvironment iweb,UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _iweb = iweb;
            _userManager = userManager;
            _context = context;
        }


        [HttpPost]
        public IActionResult TabelExcel(DateTime start, DateTime end)
        {

            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var dep = _context.Managers.Include(u => u.User).ThenInclude(d => d.Department).FirstOrDefault(i => i.ManagerId == ManId);
            var Employees = _context.Employees.Include(u => u.User).ThenInclude(d => d.Department).ToList().Where(i => i.ManagerId == ManId && i.EmployeeState != "out");
            var Reportss = _context.Reports.ToList().Where(i => i.ReportDate.Date >= start.Date && i.ReportDate.Date <= end.Date && i.ManagerId == ManId);
            var Pl = _context.Plans.ToList().Where(i => i.ManagerId == ManId);
            DateTime dayOfWeek;
            DateTime now = DateTime.Now;
            DateTime dayOfW2;
            List<int> days = new List<int>();
           var totalMonth = (end.Month - start.Month) + 12 * (end.Year - start.Year);
  
            using (var workbook = new XLWorkbook())
            {
                //var sheet = workbook.W;
                for (int item = 0; item <= totalMonth; item++)
                {
                    int k = 1;

                    int d = DateTime.DaysInMonth(start.Year, start.Month + item);
                    days.Add(d);
                  
                    DateTime month = start.AddMonths(item);
                    var sheet = workbook.Worksheets.Add(month.ToString("MMMM"));
                    sheet.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                    sheet.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    var range1 = sheet.Range("A6:A7");
                    range1.Value = "№";
                    range1.Merge();
                    range1.Style.Font.Bold = true;
                    sheet.Columns("A").Width = 4;
                    sheet.Columns("B,C").Width = 40;
                    var range2 = sheet.Range("B6:B7");
                    range2.Value = "Ф.И.О";
                    range2.Merge();
                    range2.Style.Font.FontSize = 12;
                    range2.Style.Font.Bold = true;
                    var range3 = sheet.Range("C6:C7");
                    range3.Value = "Занимаемая должность";
                    range3.Merge();
                    range3.Style.Font.FontSize = 12;
                    range3.Style.Font.Bold = true;
                    int startDay = 1;
                    int endDay = DateTime.DaysInMonth(month.Year, month.Month);
                    if (month.Month == start.Month && month.Year == start.Year)
                    {
                        startDay = start.Day;
                    }
                    if (month.Month == end.Month && month.Year == end.Year)
                    {
                        endDay = end.Day;
                    }
                    for (int i = startDay; i >= startDay && i <= endDay;)
                    {
                        dayOfWeek = new DateTime(month.Year, month.Month, i);
                        dayOfWeek.DayOfWeek.ToString();
                        if (dayOfWeek.DayOfWeek.ToString() == "Saturday" || dayOfWeek.DayOfWeek.ToString() == "Sunday")
                        {
                            sheet.Cell(7, k + 3).Style.Fill.SetBackgroundColor(XLColor.PastelGray);
                        }

                        sheet.Cell(7, k + 3).Value = i.ToString();
                        k++;
                        i++;
                    }
                    sheet.Columns(4, k + 2).Width = 4;
                    sheet.Column(k + 3).Width = 6;
                    sheet.Columns(k + 4, k + 10).Width = 4;
                    sheet.Column(k + 11).Width = 7;
                    sheet.Row(6).Height = 30;
                    sheet.Row(7).Height = 40;
                    var range4 = sheet.Range(6, 4, 6, k + 2);
                    range4.Value = "Числа месяца";
                    range4.Merge();
                    range4.Style.Font.Bold = true;

                    var range5 = sheet.Range(6, k + 3, 7, k + 3);
                    range5.Value = "Отработ. дни";
                    range5.Merge();
                    range5.Style.Alignment.TextRotation = 90;
                    range5.Style.Font.Bold = true;

                    sheet.Cell(7, k + 4).Value = "Б";
                    sheet.Cell(7, k + 5).Value = "б/с";
                    sheet.Cell(7, k + 6).Value = "К";
                    sheet.Cell(7, k + 7).Value = "У";
                    sheet.Cell(7, k + 8).Value = "О";
                    sheet.Cell(7, k + 9).Value = "Уд";
                    sheet.Cell(7, k + 10).Value = "Пр";
                    var range6 = sheet.Range(6, k + 4, 6, k + 10);
                    range6.Value = "Неявки";
                    range6.Merge();
                    range6.Style.Font.Bold = true;

                    var range7 = sheet.Range(6, k + 11, 7, k + 11);
                    range7.Value = "Отработ. часы всего";
                    range7.Merge();
                    range7.Style.Alignment.WrapText = true;
                    range7.Style.Alignment.TextRotation = 90;
                    range7.Style.Font.Bold = true;

                    var range8 = sheet.Range(2, 1, 2, k + 11);
                    range8.Value = "Табель за " + month.ToString("MMMM") + " " + month.ToString("yyyy") + "г.";
                    range8.Merge();
                    range8.Style.Font.FontSize = 14;
                    range8.Style.Font.Bold = true;


                    var range9 = sheet.Range(3, 2, 3, 3);
                    range9.Value = "Организация: АО \"Узбекнефтгаз\"";
                    range9.Merge();
                    range9.Style.Font.Bold = true;
                    range9.Style.Font.FontSize = 13;

                    var range10 = sheet.Range(5, 2, 5, 3);
                    range10.Value = dep.User.Department.DepartmentName;
                    range10.Merge();
                    range10.Style.Font.Bold = true;

                    sheet.SheetView.Freeze(7, 3);
                    int e = 8;
                    foreach (var item1 in Employees)
                    {
                        sheet.Cell(e, 1).Value = e - 7;
                        sheet.Cell(e, 2).Value = item1.User.FullName;
                        sheet.Cell(e, 2).Style.Font.FontSize = 12;
                        sheet.Cell(e, 3).Value = item1.User.Position;
                        sheet.Cell(e, 3).Style.Font.FontSize = 12;
                        for (int j = startDay; j >= startDay && j <= endDay;)
                        {
                            for (int s = 4; s <= k + 2; s++)
                            {

                                if ((now.Year == month.Year && now.Month <= month.Month && now.Day < j) || (now.Year == month.Year && now.Month < month.Month) )
                                {
                                    if (Pl.ToList().Where(i => i.EmployeeId == item1.EmployeeId) != null)
                                    {
                                        foreach (var plan in Pl.ToList().Where(i => i.EmployeeId == item1.EmployeeId))
                                        {
                                            ////************************************************
                                            if ((j >= plan.PlanStart.Day && j <= plan.PlanEnd.Day &&
                                                month.Month >= plan.PlanStart.Month && month.Month <= plan.PlanEnd.Month
                                                && month.Year == plan.PlanStart.Year && month.Year <= plan.PlanEnd.Year) || (j <= plan.PlanEnd.Day && month.Month > plan.PlanStart.Month && month.Month == plan.PlanEnd.Month
                                                && month.Year >= plan.PlanStart.Year && month.Year <= plan.PlanEnd.Year) || (month.Month > plan.PlanStart.Month && month.Month < plan.PlanEnd.Month
                                                && month.Year >= plan.PlanStart.Year && month.Year <= plan.PlanEnd.Year))
                                            {
                                                if (plan.State == "")
                                                {
                                                    sheet.Cell(e, s).Value = "-";
                                                }
                                                else
                                                {
                                                    sheet.Cell(e, s).Value = plan.State;
                                                    sheet.Cell(e, s).Style.Fill.SetBackgroundColor(XLColor.ArylideYellow);
                                                }
                                            }
                                            else
                                            {
                                                sheet.Cell(e, s).Value = "-";
                                            }

                                        }

                                    }
                                    else
                                    {
                                        sheet.Cell(e, s).Value = "-";
                                    }

                                }
                                else
                                {

                                    if (Reportss.FirstOrDefault(i => i.EmployeeId == item1.EmployeeId && i.ReportDate.Month == month.Month && i.ReportDate.Day == j) != null)
                                    {
                                        sheet.Cell(e, s).Value = Reportss.FirstOrDefault(i => i.EmployeeId == item1.EmployeeId && i.ReportDate.Month == month.Month && i.ReportDate.Day == j).State;
                                    }
                                    else
                                    {
                                        sheet.Cell(e, s).Value = "-";
                                    }

                                }


                                dayOfW2 = new DateTime(month.Year, month.Month, j);
                                if (dayOfW2.DayOfWeek.ToString() == "Saturday" || dayOfW2.DayOfWeek.ToString() == "Sunday")
                                {
                                    sheet.Cell(e, s).Style.Fill.SetBackgroundColor(XLColor.PastelGray);
                                    sheet.Cell(e, s).Value = "В";
                                }
                                j++;


                            }

                        }
                        sheet.Cell(e, k + 3).SetFormulaR1C1("=IF(COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",8) + COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",7) + COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"К\") + " +
                            "COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Уд\")=0,\"\", + COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",8) + COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",7) + COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"К\") + " +
                            "COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Уд\" ))");
                        sheet.Cell(e, k + 4).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Б\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Б\"))");
                        sheet.Cell(e, k + 5).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"б/с\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"б/с\"))");
                        sheet.Cell(e, k + 6).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"К\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"К\"))");
                        sheet.Cell(e, k + 7).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"У\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"У\"))");
                        sheet.Cell(e, k + 8).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"О\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"О\"))");
                        sheet.Cell(e, k + 9).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Уд\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Уд\"))");
                        sheet.Cell(e, k + 10).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Пр\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"Пр\"))");
                        //sheet.Cell(e, k + 11).SetFormulaR1C1("=IF(+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"И\")=0,\"\",+COUNTIF(R" + e + "C4:R" + e + "C" + (k + 2) + ",\"И\"))");
                        sheet.Cell(e, k + 11).SetFormulaR1C1("=SUM(IF(R" + e + "C" + (k + 6) + "=\"\",0,R" + e + "C" + (k + 6) + "*8) , IF(R" + e + "C" + (k + 9) + "=\"\",0, R" + e + "C" + (k + 9) + "*8), (R"+e+"C4:R"+e+"C"+(k+2)+"))");

                        e++;
                    }
                    sheet.SetShowGridLines(true);
                    var Bb = sheet.Range(6, 1, e, k + 11);
                    Bb.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                    Bb.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                    sheet.Cell(e, 2).Value = "О - Отпуск, К - Командировка, У - На учебе, Уд - Удаленка";
                    sheet.Cell(e, 3).Value = "Б - Больничный, Пр - Праздник, б/с - Отпуск без сохранения заработной платы";
                    sheet.Cell(e, 2).Style.Font.FontSize = 8;
                    sheet.Cell(e, 2).Style.Font.Bold = true;
                    sheet.Cell(e, 2).Style.Font.Italic = true;
                    sheet.Cell(e, 2).Style.Alignment.WrapText = true;
                    sheet.Cell(e, 3).Style.Font.FontSize = 8;
                    sheet.Cell(e, 3).Style.Alignment.WrapText = true;
                    sheet.Cell(e, 3).Style.Font.Bold = true;
                    sheet.Cell(e, 3).Style.Font.Italic = true;
                    sheet.Row(e).Height = 50;

                }


                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Report.xlsx"
                        );
                }

            }

        }




       




        // GET: Manager/Reports
        public async Task<IActionResult> Index(int year, int month)
        {
            List<SelectListItem> y = new List<SelectListItem>();
            foreach (var i in _context.Reports.Select(i => i.ReportDate.Year).Distinct())
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                };
                SelectListItem s = sel;
                y.Add(s);

            }
            ViewData["y"] = y;
            string n = "Все";
            List<SelectListItem> m = new List<SelectListItem>();
            foreach (var i in _context.Reports.Select(i => i.ReportDate.Month).Distinct())
            {
                if (i == 1)
                    n = "Январь";
                else if (i == 2)
                    n = "Февраль";
                else if (i == 3)
                    n = "Март";
                else if (i == 4)
                    n = "Апрель";
                else if (i == 5)
                    n = "Май";
                else if (i == 6)
                    n = "Июнь";
                else if (i == 7)
                    n = "Июль";
                else if (i == 8)
                    n = "Август";
                else if (i == 9)
                    n = "Сентябрь";
                else if (i == 10)
                    n = "Октябрь";
                else if (i == 11)
                    n = "Ноябрь";
                else if (i == 12)
                    n = "Декабрь";
                else n = "Все";
                SelectListItem sel = new SelectListItem
                {
                    Text = n,
                    Value = i.ToString(),
                };
                SelectListItem s = sel;
                m.Add(s);

            }
            ViewData["m"] = m;
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var applicationDbContext = _context.Reports.Include(r => r.Address).Include(r => r.Employee).ThenInclude(u => u.User)
                .Include(r => r.Manager).ThenInclude(u => u.User).Include(r => r.Objects).Include(r => r.Tasks) .Where(r => r.ManagerId == ManId).AsNoTracking().OrderByDescending(u => u.ReportId);
            //  var model = await PagingList.CreateAsync(applicationDbContext, 10, page);
            //  return View(model);


            if (year == 0)
            {
                var year1 = applicationDbContext.Where(i => i.ReportDate.Year == DateTime.Now.Year);
                applicationDbContext = (IOrderedQueryable<Report>)year1;
            }

            else if (year != 123 && year != 0)
            {
                var year1 = applicationDbContext.Where(i => i.ReportDate.Year == year);
                applicationDbContext = (IOrderedQueryable<Report>)year1;
            }
            if (month == 0)
            {
                var month1 = applicationDbContext.Where(i => i.ReportDate.Month == DateTime.Now.Month);
                applicationDbContext = (IOrderedQueryable<Report>)month1;
            }
            else if (month != 13 && month != 0)
            {
                var month1 = applicationDbContext.Where(i => i.ReportDate.Month == month);
                applicationDbContext = (IOrderedQueryable<Report>)month1;

            }
            ViewData["ydy"] = year;
            ViewData["mdm"] = month;

            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Index2()
        {
            DateTime now = DateTime.Today;
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var applicationDbContext = _context.Reports.Include(r => r.Address).Include(r => r.Employee).ThenInclude(u => u.User).Include(r => r.Manager).ThenInclude(u => u.User).Include(r => r.Objects).Include(r => r.Tasks)
                .Where(r => r.ManagerId == ManId && r.ReportDate.Date == now);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Manager/Reports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.Address)
                .Include(r => r.Employee).ThenInclude(r=> r.User)
                .Include(r => r.Manager).ThenInclude(r => r.User)
                .Include(r => r.Objects)
                .Include(r => r.Tasks)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            report.HaveSeen = true;
            try
            {
                _context.Update(report);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(report.ReportId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (report == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", report);
        }

        // GET: Manager/Reports/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects)
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
            return View();
        }
        [HttpPost]
        public IActionResult CreateAfterAddress(int ReportId, string State, DateTime ReportDate, string ReportDescription, string ReportComment, double ReportScore, int TaskId, int AddressId, int ObjectId, int ManagerId, int EmployeeId)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
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
            Report report = new Report
            {
                ReportId = ReportId,
                State = State,
                ReportDate = ReportDate,
                ReportDescription = ReportDescription,
                ReportComment = ReportComment,
                ReportScore = ReportScore,
                TaskId = TaskId,
                AddressId = AddressId,
                ObjectId = ObjectId,
                ManagerId = ManagerId,
                EmployeeId = EmployeeId

            };
            return View("Create", report);
        }
        [HttpPost]
        public IActionResult EditAfterAddress(int ReportId, string State, DateTime ReportDate, string ReportDescription, string ReportComment, double ReportScore, int TaskId, int AddressId, int ObjectId, int ManagerId, int EmployeeId)
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
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
            Report report = new Report
            {
                ReportId = ReportId,
                State = State,
                ReportDate = ReportDate,
                ReportDescription = ReportDescription,
                ReportComment = ReportComment,
                ReportScore = ReportScore,
                TaskId = TaskId,
                AddressId = AddressId,
                ObjectId = ObjectId,
                ManagerId = ManagerId,
                EmployeeId = EmployeeId

            };
            return View("Edit", report);
        }

        // POST: Manager/Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportId,State,ReportDate,ReportDescription,ReportComment,ReportScore,HaveSeen,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Report report)
        {
            if (report.State == "О" || report.State == "Б" || report.State == "У" || report.State == "Пр" || report.State == "б/с")
            {
                report.TaskId = null;
                report.ObjectId = null;
                report.AddressId = null;
            }
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == report.AddressId))
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
            return View(report);
        }

        // GET: Manager/Reports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FindAsync(id);
            report.HaveSeen = true;
            if (report.State == "О" || report.State == "Б" || report.State == "У" || report.State == "Пр" || report.State == "б/с")
            {
                report.TaskId = null;
                report.ObjectId = null;
                report.AddressId = null;
            }
            try
            {
                _context.Update(report);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportExists(report.ReportId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (report == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == report.AddressId))
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
            return View(report);
        }

        // POST: Manager/Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ReportId, [Bind("ReportId,State,ReportDate,ReportDescription,ReportComment,ReportScore,HaveSeen,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Report report)
        {
            if (ReportId != report.ReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(report);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReportExists(report.ReportId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == report.AddressId))
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
            return View(report);
        }

        // GET: Manager/Reports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports
                .Include(r => r.Address)
                .Include(r => r.Employee).ThenInclude(u => u.User)
                .Include(r => r.Manager).ThenInclude(u => u.User)
                .Include(r => r.Objects)
                .Include(r => r.Tasks)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Manager/Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReportExists(int id)
        {
            return _context.Reports.Any(e => e.ReportId == id);
        }
    }
}
