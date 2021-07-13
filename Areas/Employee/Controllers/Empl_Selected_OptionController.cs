using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;
using KRU.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class Empl_Selected_OptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public Empl_Selected_OptionController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Employee/Empl_Selected_Option
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Empl_Selected_Options.Include(e => e.Employee).ThenInclude(r => r.User).Include(e => e.Option).Include(e => e.Question).Include(e => e.Test);
            return View(await applicationDbContext.ToListAsync());
        }
        public List<int> GetRandomElements(IEnumerable<int> list, int elementsCount)
        {
            return list.OrderBy(x => Guid.NewGuid()).Take(elementsCount).ToList();
        }
        public async Task<IActionResult> Index2(int id)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(i => i.TestId == id);
            int count = test.RandomQuestions;
            var applicationDbContext = _context.Tests.Include(e => e.Manager).ThenInclude(p => p.User).Include(e => e.Manager)
                .ThenInclude(p => p.Employees).Include(t=>t.Department).Where(i => i.TestId ==id)
                .Include(e => e.Questions.OrderBy(x => Guid.NewGuid()).Take(count)).ThenInclude(i => i.Options);
           
            
            return View(await applicationDbContext.ToListAsync());
        }
    
        // GET: Employee/Empl_Selected_Option/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empl_Selected_Option = await _context.Empl_Selected_Options
                .Include(e => e.Employee)
                .Include(e => e.Option)
                .Include(e => e.Question)
                .Include(e => e.Test)
                .FirstOrDefaultAsync(m => m.SelectedId == id);
            if (empl_Selected_Option == null)
            {
                return NotFound();
            }

            return View(empl_Selected_Option);
        }

        // GET: Employee/Empl_Selected_Option/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["OptionId"] = new SelectList(_context.Options, "OptionId", "OptionId");
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionId");
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestId");
            return View();
        }

        // POST: Employee/Empl_Selected_Option/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SelectedId,EmployeeId,TestId,QuestionId,OptionId")] Empl_Selected_Option empl_Selected_Option)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empl_Selected_Option);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", empl_Selected_Option.EmployeeId);
            ViewData["OptionId"] = new SelectList(_context.Options, "OptionId", "OptionId", empl_Selected_Option.OptionId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionId", empl_Selected_Option.QuestionId);
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestId", empl_Selected_Option.TestId);
            return View(empl_Selected_Option);
        }

        // GET: Employee/Empl_Selected_Option/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empl_Selected_Option = await _context.Empl_Selected_Options.FindAsync(id);
            if (empl_Selected_Option == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", empl_Selected_Option.EmployeeId);
            ViewData["OptionId"] = new SelectList(_context.Options, "OptionId", "OptionId", empl_Selected_Option.OptionId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionId", empl_Selected_Option.QuestionId);
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestId", empl_Selected_Option.TestId);
            return View(empl_Selected_Option);
        }

        // POST: Employee/Empl_Selected_Option/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SelectedId,EmployeeId,TestId,QuestionId,OptionId")] Empl_Selected_Option empl_Selected_Option)
        {
            if (id != empl_Selected_Option.SelectedId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empl_Selected_Option);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Empl_Selected_OptionExists(empl_Selected_Option.SelectedId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", empl_Selected_Option.EmployeeId);
            ViewData["OptionId"] = new SelectList(_context.Options, "OptionId", "OptionId", empl_Selected_Option.OptionId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionId", empl_Selected_Option.QuestionId);
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestId", empl_Selected_Option.TestId);
            return View(empl_Selected_Option);
        }

        // GET: Employee/Empl_Selected_Option/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empl_Selected_Option = await _context.Empl_Selected_Options
                .Include(e => e.Employee)
                .Include(e => e.Option)
                .Include(e => e.Question)
                .Include(e => e.Test)
                .FirstOrDefaultAsync(m => m.SelectedId == id);
            if (empl_Selected_Option == null)
            {
                return NotFound();
            }

            return View(empl_Selected_Option);
        }

        // POST: Employee/Empl_Selected_Option/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empl_Selected_Option = await _context.Empl_Selected_Options.FindAsync(id);
            _context.Empl_Selected_Options.Remove(empl_Selected_Option);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Empl_Selected_OptionExists(int id)
        {
            return _context.Empl_Selected_Options.Any(e => e.SelectedId == id);
        }
    }
}
