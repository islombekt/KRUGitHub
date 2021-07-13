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
using Microsoft.AspNetCore.Authorization;

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> UserManager;
        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            UserManager = userManager;
            _context = context;
        }

        // GET: Manager/Employees
        public async Task<IActionResult> Index()
        {
            var Manager = _context.Managers.ToList().FirstOrDefault(u => u.UserId == UserManager.GetUserId(User)).ManagerId;
            var applicationDbContext = _context.Employees.Include(e => e.Manager).Include(e => e.User).ThenInclude(u => u.Address).Where(m => m.ManagerId == Manager && m.EmployeeState != "out" );
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Index2()
        {
            var Manager = _context.Managers.ToList().FirstOrDefault(u => u.UserId == UserManager.GetUserId(User)).ManagerId;
            var applicationDbContext = _context.Employees.Include(e => e.Manager).Include(e => e.User).ThenInclude(u => u.Address).Where(m => m.ManagerId == Manager && m.EmployeeState == "out");
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Manager/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.Include(e => e.User)
                .Include(e => e.Manager).ThenInclude(u => u.User)  
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Manager/Employees/Create
        public IActionResult Create()
        {
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return View();
        }

        // POST: Manager/Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeeState,ManagerId,UserId")] Models.Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", employee.ManagerId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id", employee.UserId);
            return View(employee);
        }

        // GET: Manager/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["ManagerId"] = new SelectList(_context.Managers.Include(u => u.User), "ManagerId", "User.FullName", employee.ManagerId);
          
            return View(employee);
        }

        // POST: Manager/Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,EmployeeState,ManagerId,UserId")] Models.Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["ManagerId"] = new SelectList(_context.Managers.Include(u => u.User), "ManagerId", "User.FullName", employee.ManagerId);
         
            return View(employee);
        }

        // GET: Manager/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Manager)
                .Include(e => e.User)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            int fc = 0;
            var fin = await _context.FinanceReports.Where(u => u.EmployeeId == employee.EmployeeId).ToListAsync();
            if (fin != null)
            {
                fc = fin.Count();
            }
            int rc = 0;
            var rep = await _context.Reports.Where(u => u.EmployeeId == employee.EmployeeId).ToListAsync();
            if (rep != null)
            {
                rc = rep.Count();
            }
            int pc = 0;
            var plan = await _context.Plans.Where(u => u.EmployeeId == employee.EmployeeId).ToListAsync();
            if (plan != null)
            {
                pc = plan.Count();
            }
            if (employee == null)
            {
                return NotFound();
            }
            KRU.Models.Employee emp = new KRU.Models.Employee
            {
                EmployeeId = employee.EmployeeId,
                EmployeeState = employee.EmployeeState,
                UserId = employee.UserId,
                ManagerId = employee.ManagerId,
                Score = employee.Score,
                FileUrl = employee.FileUrl,
                FinanceReports = fin,
                FinCount = fc,
                Reports = rep,
                RepCount = rc,
                Plans = plan,
                PlanCount = pc,
                FullName = employee.User.FullName
            };
            return View(emp);
        }

        // POST: Manager/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            var user = await _context.User.Include(u => u.Employee).FirstOrDefaultAsync(u=> u.Employee.UserId == employee.UserId);
            var role = await _context.UserRoles.FirstOrDefaultAsync(u=> u.UserId == employee.UserId);
            _context.Employees.Remove(employee);
            _context.UserRoles.Remove(role);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
