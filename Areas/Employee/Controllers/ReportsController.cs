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

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ReportsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpPost]
        public JsonResult AjaxMethod(int name)
        {
            int kech = name;
            return Json(kech);
        }
        // GET: Employee/Reports
        public async Task<IActionResult> Index()
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var applicationDbContext = _context.Reports.Include(r => r.Address).Include(r => r.Employee).Include(r => r.Manager)
                .ThenInclude(u => u.User).Include(r => r.Objects).Include(r => r.Tasks).Include(i => i.Task_Reports).ThenInclude(i => i.Tasks).Where(r => r.EmployeeId == EmpId).OrderByDescending(u => u.ReportId);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Reports/Details/5
        public async Task<IActionResult> Details(int? id)
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
                .Include(r => r.Tasks).Include(i => i.Task_Reports).ThenInclude(i => i.Tasks)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", report);
        }

        // GET: Employee/Reports/Create
        public IActionResult Create()
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var PrevRep = _context.Reports.ToList().OrderByDescending(r => r.ReportId).FirstOrDefault(r => r.EmployeeId == EmpId);
            
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
           
            //ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectName");
            if(PrevRep == null)
            {
                return View();
            }
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            if (PrevRep != null)
            {
                foreach (var itemObj in _context.Objects.Where(i => i.AddressId == PrevRep.AddressId))
                {
                    SelectListItem sel = new SelectListItem
                    {
                        Text = itemObj.ObjectName,
                        Value = itemObj.ObjectId.ToString(),
                    };
                    SelectListItem s = sel;
                    itemsObj.Add(s);
                }
            }
            ViewData["ObjectId"] = itemsObj;
            return View(PrevRep);
        }

        // POST: Employee/Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult CreateAfterAddress(int ReportId, string State,DateTime ReportDate,string ReportDescription,string ReportComment,double ReportScore,int TaskId,int AddressId,int ObjectId, int ManagerId, int EmployeeId)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            List<SelectListItem> items = new List<SelectListItem>();
            foreach(var item in _context.Objects.Where(i => i.AddressId == AddressId))
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
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
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
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
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
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
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



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportId,State,ReportDate,ReportDescription,ReportComment,ReportScore,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Report report, [Bind("Task_RepId", "CommentTR", "TaskId,RepId")] Task_Report task_rep, List<int> ListofTasks)
        {
           
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
           var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            // var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            report.EmployeeId = EmpId;
            report.ManagerId = ManId;
            report.ReportDate = DateTime.Now;
            report.TaskId = null;
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
                foreach (var i in ListofTasks)
                {
                    Task_Report rep = new Task_Report()
                    {
                        TaskId = i,
                        RepId = report.ReportId,
                        CommentTR = task_rep.CommentTR,
                    };
                    _context.Task_Reports.Add(rep);
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", report.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == report.AddressId))
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
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
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

        // GET: Employee/Reports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            if (id == null)
            {
                return NotFound();
            }
            var enm = _context.Task_Reports.Where(i => i.RepId == id).ToList().Select(i => i.TaskId);
            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", report.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == report.AddressId))
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
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
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
            report.selectedTasks = new List<int> { };
            foreach (int i in enm.ToArray())
            {
                if (i != 0)
                {
                    report.selectedTasks.Add(i);
                }
            }
            ViewBag.selectedTasks = await _context.Tasks.Select(i => i.TaskName).ToListAsync();
            return View(report);
        }

        // POST: Employee/Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportId,State,ReportDate,ReportDescription,ReportComment,ReportScore,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Report report,List<int> ListofTasks)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            // var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            report.EmployeeId = EmpId;
            report.ManagerId = ManId;
            
            if (report.State == "О" || report.State == "Б" || report.State == "У" || report.State == "Пр" || report.State == "б/с")
            {
                report.TaskId = null;
                report.ObjectId = null;
                report.AddressId = null;
            }
            if (id != report.ReportId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var task_rep = _context.Task_Reports.Where(w => w.RepId == id).ToList();
                    if (task_rep != null)
                    {
                        foreach (var i in task_rep)
                        {
                            _context.Task_Reports.Remove(i);
                        }
                    }
                    foreach (var it in ListofTasks)
                    {
                        Task_Report enT = new Task_Report()
                        {
                            RepId = report.ReportId,
                            TaskId = it,
                        };
                        _context.Task_Reports.Add(enT);
                        await _context.SaveChangesAsync();
                    }

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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", report.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == report.AddressId))
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
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
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
        // GET: Employee/Reports/Delete/5
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
                .Include(r => r.Tasks).Include(i =>i.Task_Reports).ThenInclude(i => i.Tasks)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // POST: Employee/Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            var taskRep = await _context.Task_Reports.Where(i => i.RepId == id).ToListAsync();
            if(taskRep != null)
            {
                foreach(var item in taskRep)
                {
                    _context.Task_Reports.Remove(item);
                }
            }
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
