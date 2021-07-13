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

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class Empl_Selected_OptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public Empl_Selected_OptionController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Manager/Empl_Selected_Option
        public async Task<IActionResult> Index(int? Empl, int? test)
        {
            ViewData["EmployeeId"] = Empl;
            var applicationDbContext = await _context.Questions
                .Include(i => i.Test)
                .ThenInclude(t => t.Department)
                .Include(i => i.Test)
                .ThenInclude(t => t.Manager).ThenInclude(i => i.User).Include(i => i.Options).Include(i => i.Empl_Selected_Options).Where(m => m.Test.TestId == test)
                .ToListAsync();
            if (Empl != 0 && test != 0)
            {
                var userHodim = await _context.Employees.Include(i => i.User).FirstOrDefaultAsync(i => i.EmployeeId == Empl);
                var questions = await _context.Questions
                .Include(i => i.Test)
                .ThenInclude(t => t.Department)
                .Include(i => i.Test)
                .ThenInclude(t => t.Manager).ThenInclude(i => i.User).Include(i => i.Options).Include(i => i.Empl_Selected_Options).Where(m => m.Test.TestId == test)
                .ToListAsync();
                foreach (var item in questions)
                {
                    var s = await _context.Options.FirstOrDefaultAsync(a => a.QuestionId == item.QuestionId && a.Correct == true);
                    if (s != null)
                    {
                        item.CorrectOption = s.OptName;
                    }
                    else
                    {
                        item.CorrectOption = "";
                    }
                    var selec = await _context.Empl_Selected_Options.Include(i => i.Option).Include(i => i.Question).FirstOrDefaultAsync(a => a.QuestionId == item.QuestionId && a.EmployeeId == Empl);
                    if (selec != null)
                    {
                        item.UserSelectedOption = selec.Option.OptName;
                        item.IsUserSelectedOptionCorrect = selec.Option.Correct;
                    }
                    else
                    {
                        item.UserSelectedOption = "";
                        item.IsUserSelectedOptionCorrect = false;
                    }
                    item.UserName = userHodim.User.FullName;

                }

                if (questions == null)
                {
                    return NotFound();
                }

                return View(questions);


            }
            return View(applicationDbContext);
        }

        public async Task<IActionResult> Index2(int? id)
        {
            ViewData["tst"] = id;
            var testRes = await _context.Empl_Selected_Options.Include(i => i.Test).Where(i => i.TestId == id).Include(i => i.Question).ThenInclude(i => i.Options).ToListAsync();
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var applicationDbContext = await _context.Employees.Where(i => i.ManagerId == ManId).Include(i => i.User).ToListAsync();
            foreach (var item in applicationDbContext)
            {
                if (testRes == null)
                {
                    item.RepCount = 0;
                    item.FinCount = id;
                }
                else
                {
                    item.RepCount = testRes.Where(i => i.Option.Correct == true && i.EmployeeId == item.EmployeeId).Select(i => i.OptionId).Distinct().Count();
                    item.FinCount = id;
                }

            }
            return View(applicationDbContext);
        }

        // GET: Manager/Empl_Selected_Option/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empl_Selected_Option = await _context.Empl_Selected_Options
                .Include(e => e.Employee).ThenInclude(u => u.User)
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

        // GET: Manager/Empl_Selected_Option/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["OptionId"] = new SelectList(_context.Options, "OptionId", "OptionId");
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QuestionId");
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestId");
            return View();
        }

        // POST: Manager/Empl_Selected_Option/Create
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
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QName", empl_Selected_Option.QuestionId);
            ViewData["TestId"] = new SelectList(_context.Tests, "TestId", "TestName", empl_Selected_Option.TestId);
            return View(empl_Selected_Option);
        }

        // GET: Manager/Empl_Selected_Option/Edit/5
        public async Task<IActionResult> Edit(int? Qid, int? EmpId)
        {
            if (Qid == null || EmpId ==null)
            {
                return NotFound();
            }

            var empl_Selected_Option = await _context.Empl_Selected_Options.FirstOrDefaultAsync(i => i.QuestionId == Qid && i.EmployeeId==EmpId);
            if (empl_Selected_Option == null)
            {
                return NotFound();
            }
            ViewData["Select"] = empl_Selected_Option.SelectedId;
            ViewData["Empl"] =EmpId;
            ViewData["Tst"] = empl_Selected_Option.TestId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(i => i.User).Where(i => i.EmployeeId == EmpId), "EmployeeId", "User.FullName", empl_Selected_Option.EmployeeId);
            ViewData["OptionId"] = new SelectList(_context.Options.Where(i => i.QuestionId == Qid), "OptionId", "OptName", empl_Selected_Option.OptionId);
            ViewData["QuestionId"] = new SelectList(_context.Questions.Where(i => i.QuestionId == Qid), "QuestionId", "QName", empl_Selected_Option.QuestionId);
            ViewData["TestId"] = new SelectList(_context.Tests.Where(i =>i.TestId == empl_Selected_Option.TestId), "TestId", "TestName", empl_Selected_Option.TestId);
            return View(empl_Selected_Option);
        }

        // POST: Manager/Empl_Selected_Option/Edit/5
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
                return RedirectToAction("Index", "Empl_Selected_Option", new { Empl = empl_Selected_Option.EmployeeId, test = empl_Selected_Option.TestId });
            }
            ViewData["Select"] = empl_Selected_Option.SelectedId;
            ViewData["Empl"] = empl_Selected_Option.EmployeeId;
            ViewData["Tst"] = empl_Selected_Option.TestId;
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Include(i => i.User).Where(i => i.EmployeeId == empl_Selected_Option.EmployeeId), "EmployeeId", "User.FullName", empl_Selected_Option.EmployeeId);
            ViewData["OptionId"] = new SelectList(_context.Options.Where(i => i.QuestionId == empl_Selected_Option.QuestionId), "OptionId", "OptName", empl_Selected_Option.OptionId);
            ViewData["QuestionId"] = new SelectList(_context.Questions.Where(i => i.QuestionId == empl_Selected_Option.QuestionId), "QuestionId", "QName", empl_Selected_Option.QuestionId);
            ViewData["TestId"] = new SelectList(_context.Tests.Where(i => i.TestId == empl_Selected_Option.TestId), "TestId", "TestName", empl_Selected_Option.TestId);
            return View(empl_Selected_Option);
        }

        // GET: Manager/Empl_Selected_Option/Delete/5
        public async Task<IActionResult> Delete(int? Qid, int? EmpId)
        {
            if (Qid == null || EmpId == null)
            {
                return NotFound();
            }
           
            var empl_Selected_Option = await _context.Empl_Selected_Options
                .Include(e => e.Employee).ThenInclude(u => u.User)
                .Include(e => e.Option)
                .Include(e => e.Question)
                .Include(e => e.Test)
                .FirstOrDefaultAsync(m => m.QuestionId == Qid && m.EmployeeId == EmpId);
            if (empl_Selected_Option == null)
            {
                return NotFound();
            }
            ViewData["Sel"] = empl_Selected_Option.SelectedId;
            ViewData["Empl"] = EmpId;
            ViewData["Tst"] = empl_Selected_Option.TestId;
            return View(empl_Selected_Option);
        }

        // POST: Manager/Empl_Selected_Option/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empl_Selected_Option = await _context.Empl_Selected_Options.FindAsync(id);
            _context.Empl_Selected_Options.Remove(empl_Selected_Option);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Empl_Selected_Option", new { Empl = empl_Selected_Option.EmployeeId, test = empl_Selected_Option.TestId });
        }

        private bool Empl_Selected_OptionExists(int id)
        {
            return _context.Empl_Selected_Options.Any(e => e.SelectedId == id);
        }
    }
}
