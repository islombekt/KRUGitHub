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
    public class PlansController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public PlansController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Employee/Plans
        public async Task<IActionResult> Index()
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            var applicationDbContext = _context.Plans.Include(p => p.Address).Include(p => p.Employee).Include(p => p.Manager)
                .Include(p => p.Objects).Include(p => p.Tasks).Where(r => r.EmployeeId == EmpId).OrderByDescending(u => u.PlanId); 
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Plans/Details/5
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
            if (plan == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", plan);
        }

        // GET: Employee/Plans/Create
        public IActionResult Create()
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
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
            return View();
        }
        [HttpPost]
        public IActionResult CreateAfterAddress(int PlanId,  DateTime PlanStart, DateTime PlanEnd, string PlanDescription,  int TaskId, int AddressId, int ObjectId, int ManagerId, int EmployeeId, string State)
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
        // POST: Employee/Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,PlanStart,PlanEnd,PlanDescription,TaskId,AddressId,ObjectId,ManagerId,EmployeeId,State")] Plan plan)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            plan.EmployeeId = EmpId;
            plan.ManagerId = ManId;
            if(plan.State == "О" || plan.State == "Б" || plan.State == "У" || plan.State == "Пр" || plan.State == "б/с")
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", plan.EmployeeId);
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
            return View(plan);
        }

        // GET: Employee/Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", plan.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", plan.EmployeeId);
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
            return View(plan);
        }
        [HttpPost]
        public IActionResult EditAfterAddress(int PlanId, DateTime PlanStart, DateTime PlanEnd, string PlanDescription, int TaskId, int AddressId, int ObjectId, int ManagerId, int EmployeeId, string State)
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

        // POST: Employee/Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlanId,PlanStart,PlanEnd,PlanDescription,TaskId,AddressId,ObjectId,ManagerId,EmployeeId,State")] Plan plan)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;

            if (id != plan.PlanId)
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", plan.EmployeeId);
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
            return View(plan);
        }

        // GET: Employee/Plans/Delete/5
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

        // POST: Employee/Plans/Delete/5
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
