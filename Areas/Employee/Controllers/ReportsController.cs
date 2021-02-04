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
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee/Reports
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reports.Include(r => r.Address).Include(r => r.Employee).Include(r => r.Manager).Include(r => r.Objects).Include(r => r.Tasks);
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
                .Include(r => r.Employee)
                .Include(r => r.Manager)
                .Include(r => r.Objects)
                .Include(r => r.Tasks)
                .FirstOrDefaultAsync(m => m.ReportId == id);
            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        // GET: Employee/Reports/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId");
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId");
            return View();
        }

        // POST: Employee/Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReportId,State,ReportDate,ReportDescription,ReportComment,ReportScore,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Report report)
        {
            if (ModelState.IsValid)
            {
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", report.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId", report.ObjectId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", report.TaskId);
            return View(report);
        }

        // GET: Employee/Reports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var report = await _context.Reports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", report.AddressId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", report.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId", report.ObjectId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", report.TaskId);
            return View(report);
        }

        // POST: Employee/Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReportId,State,ReportDate,ReportDescription,ReportComment,ReportScore,TaskId,AddressId,ObjectId,ManagerId,EmployeeId")] Report report)
        {
            if (id != report.ReportId)
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", report.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", report.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectId", report.ObjectId);
            ViewData["TaskId"] = new SelectList(_context.Tasks, "TaskId", "TaskId", report.TaskId);
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
                .Include(r => r.Employee)
                .Include(r => r.Manager)
                .Include(r => r.Objects)
                .Include(r => r.Tasks)
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
