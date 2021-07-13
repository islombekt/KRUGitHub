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

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class Category111Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Category111Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager/Category111
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Category111s.Include(c => c.Category11).ThenInclude(c => c.Category1);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/Category111/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category111 = await _context.Category111s
                .Include(c => c.Category11)
                .FirstOrDefaultAsync(m => m.Category111Id == id);
            var fin = await _context.FinanceReports.Where(u => u.Category111Id == id).ToListAsync();
            if (category111 == null)
            {
                return NotFound();
            }
            Category111 category = new Category111
            {
                Category111Id = category111.Category111Id,
                Name_C111 = category111.Name_C111,
                Category11 = category111.Category11,
                FinanceReports = fin,
            };
            return View(category);
        }

        // GET: Manager/Category111/Create
        public IActionResult Create()
        {
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11");
            return View();
        }

        // POST: Manager/Category111/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category111Id,Name_C111,Category11Id")] Category111 category111)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category111);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Category11", new { id = category111.Category11Id });
            }
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", category111.Category11Id);
            return View(category111);
        }

        // GET: Manager/Category111/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category111 = await _context.Category111s.FindAsync(id);
            if (category111 == null)
            {
                return NotFound();
            }
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", category111.Category11Id);
            return View(category111);
        }

        // POST: Manager/Category111/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Category111Id,Name_C111,Category11Id")] Category111 category111)
        {
            if (id != category111.Category111Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category111);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Category111Exists(category111.Category111Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Category11", new { id = category111.Category11Id });
            }
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", category111.Category11Id);
            return View(category111);
        }

        // GET: Manager/Category111/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category111 = await _context.Category111s
                .Include(c => c.Category11)
                .FirstOrDefaultAsync(m => m.Category111Id == id);
            var fin = await _context.FinanceReports.Where(u => u.Category111Id == id).ToListAsync();
            if (category111 == null)
            {
                return NotFound();
            }
            int b = 0;
            if (fin != null)
            {
                b = fin.Count();
            }
            Category111 category = new Category111
            {
                Category111Id = category111.Category111Id,
                Name_C111 = category111.Name_C111,
                Category11 = category111.Category11,
              
                FinanceReports = fin,
                FinCount = b
            };
            return View(category);
        }

        // POST: Manager/Category111/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category111 = await _context.Category111s.FindAsync(id);
            _context.Category111s.Remove(category111);
            await _context.SaveChangesAsync();
            return RedirectToAction("Delete", "Category11", new { id = category111.Category11Id });
        }

        private bool Category111Exists(int id)
        {
            return _context.Category111s.Any(e => e.Category111Id == id);
        }
    }
}
