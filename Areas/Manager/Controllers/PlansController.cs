using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using ClosedXML.Excel;
using System.IO;

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class PlansController : Controller
    {
        private readonly IWebHostEnvironment _iweb;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public PlansController(IWebHostEnvironment iweb, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _iweb = iweb;
            _userManager = userManager;
            _context = context;
        }


        [HttpPost]
        public IActionResult KunlikExcel(DateTime start, DateTime end)
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
            string sheetPrevValue = "";


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

                   
                   

                    var range8 = sheet.Range(2, 1, 2, k + 11);
                    range8.Value =  month.ToString("MMMM") + " " + month.ToString("yyyy") + "г.";
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

                                if (now.Year == month.Year && now.Month == month.Month && now.Day < j)
                                {
                                    if (Pl.ToList().Where(i => i.EmployeeId == item1.EmployeeId) != null)
                                    {
                                        foreach (var plan in Pl.ToList().Where(i => i.EmployeeId == item1.EmployeeId))
                                        {
                                            ////************************************************
                                            if (j >= plan.PlanStart.Day && j <= plan.PlanEnd.Day &&
                                                month.Month >= plan.PlanStart.Month && month.Month <= plan.PlanEnd.Month
                                                && month.Year >= plan.PlanStart.Year && month.Year <= plan.PlanEnd.Year)
                                            {
                                                if (plan.PlanDescription == "")
                                                {
                                                    sheet.Cell(e, s).Value = "-";
                                                }
                                                else
                                                {
                                                    sheetPrevValue = sheet.Cell(e, s).Value.ToString();
                                                    sheet.Cell(e, s).Value =sheetPrevValue + " # " + plan.PlanDescription;
                                                    sheet.Column(s).Width = 40;

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
                                       
                                        foreach(var i in Reportss.Where(i => i.EmployeeId == item1.EmployeeId && i.ReportDate.Month == month.Month && i.ReportDate.Day == j))
                                        {
                                            sheetPrevValue = sheet.Cell(e, s).Value.ToString();
                                            sheet.Cell(e, s).Value = sheetPrevValue + " # " + i.ReportDescription;
                                        }
                                        sheet.Column(s).Width = 40;
                                        
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
                                    sheet.Column(s).Width = 5;
                                }
                                j++;


                            }

                        }
                      
                        e++;
                    }
                    sheet.SetShowGridLines(true);
                    var Bb = sheet.Range(6, 1, e, k +2);
                    Bb.Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                    Bb.Style.Border.SetOutsideBorder(XLBorderStyleValues.Double);
                    Bb.Style.Alignment.WrapText = true;
                    sheet.Cell(e, 2).Style.Font.FontSize = 8;
                    sheet.Cell(e, 2).Style.Font.Bold = true;
                    sheet.Cell(e, 2).Style.Font.Italic = true;
                    sheet.Cell(e, 2).Style.Alignment.WrapText = true;
                    sheet.Cell(e, 3).Style.Font.FontSize = 8;
                    sheet.Cell(e, 3).Style.Alignment.WrapText = true;
                    sheet.Cell(e, 3).Style.Font.Bold = true;
                    sheet.Cell(e, 3).Style.Font.Italic = true;
                    sheet.Row(e).Height = 50;
                    sheet.Rows(8,e).Height = 50;
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


        // GET: Manager/Plans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Plans.Include(p => p.Address).Include(p => p.Employee).ThenInclude(u => u.User).Include(p => p.Manager)
                .ThenInclude(u => u.User).Include(p => p.Objects).Include(p => p.Tasks).OrderByDescending(u => u.PlanId); 
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .Include(p => p.Address)
                .Include(p => p.Employee).ThenInclude(u => u.User)
                .Include(p => p.Manager).ThenInclude(u => u.User)
                .Include(p => p.Objects)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(m => m.PlanId == id);
            plan.HaveSeen = true;
            try
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(plan.PlanId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (plan == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", plan);
        }

        // GET: Manager/Plans/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == _context.Addresses.FirstOrDefault().AddressId))
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
            return View();
        }
        [HttpPost]
        public IActionResult CreateAfterAddress(int PlanId, DateTime PlanStart, DateTime PlanEnd, string PlanDescription, int TaskId, int AddressId, int ObjectId, int ManagerId, int EmployeeId, string State)
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
            Plan plan = new Plan
            {
                PlanId = PlanId,
                PlanStart = PlanStart,
                PlanEnd = PlanEnd,
                PlanDescription = PlanDescription,
                TaskId = TaskId,
                AddressId = AddressId,
                ObjectId = ObjectId,
                ManagerId = ManagerId,
                EmployeeId = EmployeeId

            };
            return View("Create", plan);
        }

        // POST: Manager/Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,PlanStart,PlanEnd,PlanDescription,TaskId,AddressId,ObjectId,ManagerId,EmployeeId,State")] Plan plan)
        {
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            plan.ManagerId = ManId;
            if (plan.State == "О" || plan.State == "Б" || plan.State == "У" || plan.State == "Пр" || plan.State == "б/с")
            {
                plan.TaskId = null;
                plan.ObjectId = null;
                plan.AddressId = null;
            }
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", plan.AddressId);
           
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", plan.ManagerId);
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == plan.AddressId))
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
            return View(plan);
        }

        // GET: Manager/Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans.FindAsync(id);
            plan.HaveSeen = true;
            try
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(plan.PlanId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (plan == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", plan.AddressId);
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", plan.ManagerId);
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == plan.AddressId))
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
            return View(plan);
        }
        [HttpPost]
        public IActionResult EditAfterAddress(int PlanId, DateTime PlanStart, DateTime PlanEnd, string PlanDescription, int TaskId, int AddressId, int ObjectId, int ManagerId, int EmployeeId, string State)
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
            Plan plan = new Plan
            {
                PlanId = PlanId,
                PlanStart = PlanStart,
                PlanEnd = PlanEnd,
                PlanDescription = PlanDescription,
                TaskId = TaskId,
                AddressId = AddressId,
                ObjectId = ObjectId,
                ManagerId = ManagerId,
                EmployeeId = EmployeeId

            };
            return View("Edit", plan);
        }

        // POST: Manager/Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int PlanId, [Bind("PlanId,PlanStart,State,PlanEnd,PlanDescription,TaskId,AddressId,ObjectId,ManagerId,EmployeeId,HaveSeen")] Plan plan)
        {
           
            if (PlanId != plan.PlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (plan.State == "О" || plan.State == "Б" || plan.State == "У" || plan.State == "Пр" || plan.State == "б/с")
                    {
                        plan.TaskId = null;
                        plan.ObjectId = null;
                        plan.AddressId = null;
                    }
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.PlanId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", plan.AddressId);
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(u => u.User).Where(i => i.ManagerId == ManId), "EmployeeId", "User.FullName");

            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", plan.ManagerId);
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == plan.AddressId))
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
            return View(plan);
        }

        // GET: Manager/Plans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .Include(p => p.Address)
                .Include(p => p.Employee).ThenInclude(u => u.User)
                .Include(p => p.Manager).ThenInclude(u => u.User)
                .Include(p => p.Objects)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(m => m.PlanId == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Manager/Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.Plans.FindAsync(id);
            _context.Plans.Remove(plan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanExists(int id)
        {
            return _context.Plans.Any(e => e.PlanId == id);
        }
    }
}
