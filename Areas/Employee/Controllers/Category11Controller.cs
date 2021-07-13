using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class Category11Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Category11Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee/Category11
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Category11s.Include(c => c.Category1);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Category11/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category11 = await _context.Category11s
                .Include(c => c.Category1).Include(u => u.FinanceReports).ThenInclude(u => u.Employee).ThenInclude(u => u.User).Include(u => u.FinanceReports).ThenInclude(u => u.Manager).ThenInclude(u => u.User)

                .FirstOrDefaultAsync(m => m.Category11Id == id);
            int b = 0;
            var category111 = await _context.Category111s.Where(m => m.Category11Id == id).ToListAsync();
            var fin = await _context.FinanceReports.Where(u => (u.Category11Id == id && u.Category111Id==null)).ToListAsync();
            if (category111 != null)
            {
                b = category111.Count();
            }
            if (category11 == null)
            {
                return NotFound();
            }
            Category11 category = new Category11
            {
                Category11Id = category11.Category11Id,
                Name_C11 = category11.Name_C11,
                Category1Id = category11.Category1Id,
                Category111s = category111,
                Count111 = b,
                FinanceReports = fin,
            };
            return View(category);
        }

        // GET: Employee/Category11/Create
        public IActionResult Create()
        {
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            return View();
        }

        // POST: Employee/Category11/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category11Id,Name_C11,Category1Id")] Category11 category11)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category11);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Category1", new { id = category11.Category1Id });
            }
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", category11.Category1Id);
            return View(category11);
        }

        // GET: Employee/Category11/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category11 = await _context.Category11s.FindAsync(id);
            if (category11 == null)
            {
                return NotFound();
            }
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", category11.Category1Id);
            return View(category11);
        }

        // POST: Employee/Category11/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Category11Id,Name_C11,Category1Id")] Category11 category11)
        {
            if (id != category11.Category11Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category11);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Category11Exists(category11.Category11Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Category1", new { id = category11.Category1Id });
            }
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Category1Id", category11.Category1Id);
            return View(category11);
        }

        // GET: Employee/Category11/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category11 = await _context.Category11s
                .Include(c => c.Category1)
                .FirstOrDefaultAsync(m => m.Category11Id == id);
            int b = 0;
            int a = 0;
            var category111 = await _context.Category111s.Where(m => m.Category11Id == id).ToListAsync();
            var fin = await _context.FinanceReports.Where(u => (u.Category11Id == id && u.Category111Id == null)).ToListAsync();
            if (category111 != null)
            {
                b = category111.Count();
            }
            if (fin != null)
            {
                a = fin.Count();
            }
            if (category11 == null)
            {
                return NotFound();
            }
            Category11 category = new Category11
            {
                Category11Id = category11.Category11Id,
                Name_C11 = category11.Name_C11,
                Category1Id = category11.Category1Id,
                Category111s = category111,
                Count111 = b,
                FinanceReports = fin,
                FinCount = a
            };
            return View(category);
          
        }

        // POST: Employee/Category11/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category11 = await _context.Category11s.FindAsync(id);
            _context.Category11s.Remove(category11);
            await _context.SaveChangesAsync();
            return RedirectToAction("Delete", "Category1", new { id = category11.Category1Id });
        }

        private bool Category11Exists(int id)
        {
            return _context.Category11s.Any(e => e.Category11Id == id);
        }
    }
}
