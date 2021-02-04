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

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class PlansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee/Plans
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Plans.Include(p => p.Address).Include(p => p.Employee).Include(p => p.Manager).Include(p => p.Objects).Include(p => p.Tasks);
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
                .Include(p => p.Employee)
                .Include(p => p.Manager)
                .Include(p => p.Objects)
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(m => m.PlanId == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // GET: Employee/Plans/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId");
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId");
            return View();
        }

        // POST: Employee/Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,PlanStart,PlanEnd,PlanDescription,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Plan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", plan.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", plan.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", plan.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId", plan.ObjectId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", plan.TaskId);
            return View(plan);
        }

        // GET: Employee/Plans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
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
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId", plan.ObjectId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", plan.TaskId);
            return View(plan);
        }

        // POST: Employee/Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlanId,PlanStart,PlanEnd,PlanDescription,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Plan plan)
        {
            if (id != plan.PlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", plan.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", plan.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", plan.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId", plan.ObjectId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", plan.TaskId);
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
                .Include(p => p.Employee)
                .Include(p => p.Manager)
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
