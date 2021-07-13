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
using Newtonsoft.Json;

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = SD.Role_Employee)]
    public class TestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public TestsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> PostTests([FromBody] dynamic listObj)
        {
            dynamic stuff = listObj;
            
            
            var EmplId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            foreach (var iss in stuff)
            {
                
                var q = await _context.Questions.ToListAsync();
                // testId = q.FirstOrDefault(s => s.QuestionId == iss.questionId).TestId;
                Empl_Selected_Option selected_Option = new Empl_Selected_Option() { 
                TestId = iss.testId,
                EmployeeId = EmplId,
                QuestionId = iss.questionId,
                OptionId = iss.optionId,
            };
                if (ModelState.IsValid)
                {
                    _context.Add(selected_Option);
                    await _context.SaveChangesAsync();

                }
             
                else
                {
                    return View();
                }
               
            }

            //  return Json(new { redirectToUrl = Url.Action("Index", "Tests") });
            return RedirectToAction(nameof(Details));

        }

        [HttpGet]
        public JsonResult GetTasks()
        {
            int count = 0;
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var tests = _context.Tests.Where(r => r.ManagerId == ManId && r.TestStarted.Date <= DateTime.Now.Date && r.TestEnd.Date >= DateTime.Now.Date);
            foreach(var ts in tests)
            {
                if(_context.Empl_Selected_Options.Where(i => i.TestId == ts.TestId).ToList().Select(i => i.EmployeeId).Contains(EmpId))
                {

                }
                else
                {
                    count++;
                }
            }

          
            return Json(count);
        }
      
        // GET: Employee/Tests
        public async Task<IActionResult> Index()
        {
            
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            //var applicationDbContext = _context.Tests.Include(t => t.Department).Include(t => t.Manager).Where(r => r.ManagerId == ManId && r.TestStarted.Date <= DateTime.Now.Date && r.TestEnd.Date >= DateTime.Now.Date).OrderByDescending(i => i.TestId);
            var applicationDbContext = _context.Tests.Include(t => t.Department).Include(t => t.Manager).Include(i => i.Questions).ThenInclude(t => t.Options).Where(r => r.ManagerId == ManId && r.TestStarted.Date <= DateTime.Now.Date).OrderByDescending(i => i.TestId);

            foreach (var item in applicationDbContext)
            {
                if (_context.Empl_Selected_Options.Where(i => i.TestId == item.TestId).ToList().Select(i => i.EmployeeId).Contains(EmpId))
                {
                    item.End = true;
                    var ssd = await _context.Empl_Selected_Options.Include(i => i.Option).Where(i => i.TestId == item.TestId && i.EmployeeId == EmpId && i.Option.Correct == true).Select(i => i.OptionId).Distinct().ToListAsync();
                    
                    item.ScoreOfUser = ssd.Count();

                }
                else if(item.TestEnd <= DateTime.Now.Date)
                {
                    item.End = true;
                }
            

            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Employee/Tests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var questions = await _context.Questions
                .Include(i => i.Test)
                .ThenInclude(t => t.Department)
                .Include(i => i.Test)
                .ThenInclude(t => t.Manager).ThenInclude(i => i.User).Include(i => i.Options).Include(i => i.Empl_Selected_Options).Where(m => m.Test.TestId == id)
                .ToListAsync();
            foreach(var item in questions)
            {
                var s = await _context.Options.FirstOrDefaultAsync(a => a.QuestionId == item.QuestionId && a.Correct == true);
                if(s != null)
                {
                    item.CorrectOption = s.OptName;
                }
                else
                {
                    item.CorrectOption = "";
                }
                var selec = await _context.Empl_Selected_Options.Include(i => i.Option).Include(i => i.Question).FirstOrDefaultAsync(a => a.QuestionId == item.QuestionId && a.EmployeeId == EmpId);
                if(selec != null)
                {
                    item.UserSelectedOption = selec.Option.OptName;
                    item.IsUserSelectedOptionCorrect = selec.Option.Correct;
                }
                else
                {
                    item.UserSelectedOption = "";
                    item.IsUserSelectedOptionCorrect = false;
                }
               
            }
            if (questions == null)
            {
                return NotFound();
            }

            return View(questions);
        }

        // GET: Employee/Tests/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            return View();
        }

        // POST: Employee/Tests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestId,TestName,RandomQuestions,TestDescription,TestStarted,TestEnd,Time,ManagerId,DepartmentId")] Test test)
        {
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

        // GET: Employee/Tests/Edit/5
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

        // POST: Employee/Tests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TestId,TestName,RandomQuestions,TestDescription,TestStarted,TestEnd,Time,ManagerId,DepartmentId")] Test test)
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

        // GET: Employee/Tests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Department)
                .Include(t => t.Manager)
                .FirstOrDefaultAsync(m => m.TestId == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: Employee/Tests/Delete/5
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
