using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;

namespace KRU.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class Category1Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Category1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Category1
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Category1s.Include(c => c.Department);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Category1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category1 = await _context.Category1s
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Category1Id == id);
            if (category1 == null)
            {
                return NotFound();
            }

            return View(category1);
        }

        // GET: Admin/Category1/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName");
            return View();
        }

        // POST: Admin/Category1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category1Id,Name_C1,DepartmentId")] Category1 category1)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", category1.DepartmentId);
            return View(category1);
        }

        // GET: Admin/Category1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category1 = await _context.Category1s.FindAsync(id);
            if (category1 == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", category1.DepartmentId);
            return View(category1);
        }

        // POST: Admin/Category1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Category1Id,Name_C1,DepartmentId")] Category1 category1)
        {
            if (id != category1.Category1Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category1);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Category1Exists(category1.Category1Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentName", category1.DepartmentId);
            return View(category1);
        }

        // GET: Admin/Category1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category1 = await _context.Category1s
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Category1Id == id);
            if (category1 == null)
            {
                return NotFound();
            }

            return View(category1);
        }

        // POST: Admin/Category1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category1 = await _context.Category1s.FindAsync(id);
            _context.Category1s.Remove(category1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Category1Exists(int id)
        {
            return _context.Category1s.Any(e => e.Category1Id == id);
        }
    }
}
