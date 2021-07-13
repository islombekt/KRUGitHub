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

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class Category1Controller : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public Category1Controller(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Manager/Category1
        public async Task<IActionResult> Index()
        {
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            var applicationDbContext = _context.Category1s.Include(c => c.Department).Include(i => i.Category11s).ThenInclude(c => c.Category111s).Where(i => i.DepartmentId == DepId);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> Index1()
        {
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            var applicationDbContext = _context.Category1s.Include(c => c.Department).Include(i => i.Category11s).ThenInclude(c => c.Category111s).Where(i => i.DepartmentId == DepId);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Manager/Category1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category1 = await _context.Category1s
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Category1Id == id);
            var category11 = await _context.Category11s.Where(m => m.Category1Id == id).ToListAsync();
            int a = 0;
            int fc = 0;
            var fin = await _context.FinanceReports.Where(u => (u.Category1Id == id && u.Category11Id == null)).ToListAsync();
            if (category11 != null)
            {
                a = category11.Count();
            }
            if (fin != null)
            {
                fc = fin.Count();
            }
            if (category1 == null)
            {
                return NotFound();
            }
            Category1 c = new Category1() { };
            
            Category1 category = new Category1
            {
                
                Category1Id = category1.Category1Id,
                Name_C1 = category1.Name_C1,
                Category11s = category11,
                count11 = a,
                FinanceReports = fin,
                FinCount = fc
            };
            return View(category);
        }

        // GET: Manager/Category1/Create
        public IActionResult Create()
        {

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            return View();
        }

        // POST: Manager/Category1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Category1Id,Name_C1,DepartmentId")] Category1 category1)
        {
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            if (ModelState.IsValid)
            {
                category1.DepartmentId = DepId;
                _context.Add(category1);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", category1.DepartmentId);
            return View(category1);
        }

        // GET: Manager/Category1/Edit/5
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", category1.DepartmentId);
            return View(category1);
        }

        // POST: Manager/Category1/Edit/5
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", category1.DepartmentId);
            return View(category1);
        }

        // GET: Manager/Category1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category1 = await _context.Category1s
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Category1Id == id);
            int a = 0;
            var category11 = await _context.Category11s.Where(m => m.Category1Id == id).ToListAsync();
            var fin = await _context.FinanceReports.Where(u => (u.Category1Id == id && u.Category11Id == null)).ToListAsync();
            int b = 0;
            if (category11 != null)
            {
                a = category11.Count();
            }
            if (fin != null)
            {
                b = fin.Count();
            }
            if (category1 == null)
            {
                return NotFound();
            }
            Category1 category = new Category1
            {
                Category1Id = category1.Category1Id,
                Name_C1 = category1.Name_C1,
                Category11s = category11,
                count11 = a,
                FinanceReports = fin,
                FinCount = b
            };
            return View(category);
        }

        // POST: Manager/Category1/Delete/5
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
