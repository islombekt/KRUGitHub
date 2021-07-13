using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Collections;

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public TestsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Manager/Tests
        public async Task<IActionResult> Index()
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var applicationDbContext = _context.Tests.Include(t => t.Department).Include(t => t.Manager).Where(i => i.ManagerId == ManId);
           foreach(var uf in applicationDbContext)
            {
                uf.NumberOfUsers = _context.Empl_Selected_Options.Where(i => i.TestId == uf.TestId).Select(i => i.EmployeeId).Distinct().Count();
            }           
                
           return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/Tests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
          
            var ManId = await _context.Managers.FirstOrDefaultAsync(u => u.UserId == _userManager.GetUserId(User));
            var Empl = await _context.Employees.Where(i => i.ManagerId == ManId.ManagerId).ToListAsync();
            List<int> score = new List<int>();
            if (id == null)
            {
                return NotFound();
            }
            dynamic model = new ExpandoObject();
            var test = await _context.Tests
                .Include(t => t.Department)
                .Include(t => t.Manager).ThenInclude(t => t.User).Include(i=>i.Questions).ThenInclude(i => i.Options)
                .FirstOrDefaultAsync(m => m.TestId == id);
            var passed = await _context.Empl_Selected_Options.Where(i => i.TestId == id).Select(i => i.EmployeeId).Distinct().ToListAsync();
            var question = await _context.Questions.Include(t => t.Test).Where(t => t.TestId == id).ToListAsync();
            foreach(int iE in passed)
            {
                if (_context.Empl_Selected_Options.Where(i => i.TestId == id).ToList().Select(i => i.EmployeeId).Contains(iE))
                {
                    var ssd = await _context.Empl_Selected_Options.Include(i => i.Option).Where(i => i.TestId == id && i.EmployeeId == iE && i.Option.Correct == true).Select(i => i.OptionId).Distinct().ToListAsync();

                   score.Add(ssd.Count());
                }
            }
            test.NumberOfPassedUsers = passed.Count();
            test.NumberOfUsers = Empl.Count();
            if (score.Count() == 0)
            {
                test.ScoreOfUser = 0;
                test.MinScoreOfUser = 0;
            }
          
            else
            {
                test.ScoreOfUser = score.Max();
                test.MinScoreOfUser = score.Min();
            }
           
            model.Test = test;
            model.Questions = question;
            
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // GET: Manager/Tests/Create
        public IActionResult Create()
        {
            
            //ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            //ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            return View();
        }

        // POST: Manager/Tests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestId,TestName,TestDescription,TestStarted,TestEnd,ManagerId,DepartmentId,Time,RandomQuestions")] Test test)
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            test.ManagerId = ManId;
            test.DepartmentId = DepId;
            if (ModelState.IsValid)
            {
                _context.Add(test);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", test.DepartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", test.ManagerId);
            return View(test);
        }

        // GET: Manager/Tests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", test.DepartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", test.ManagerId);
            return View(test);
        }

        // POST: Manager/Tests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TestId,TestName,TestDescription,TestStarted,TestEnd,ManagerId,Time,DepartmentId,RandomQuestions")] Test test)
        {
            if (id != test.TestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(test);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.TestId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", test.DepartmentId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", test.ManagerId);
            return View(test);
        }

        // GET: Manager/Tests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Department)
                .Include(t => t.Manager).ThenInclude(u=> u.User)
                .FirstOrDefaultAsync(m => m.TestId == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: Manager/Tests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.TestId == id);
        }
    }
}
