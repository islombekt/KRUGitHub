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
    public class OptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OptionsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Manager/Options
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Options.Include(o => o.Question);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/Options/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Options
                .Include(o => o.Question)
                .FirstOrDefaultAsync(m => m.OptionId == id);
            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        // GET: Manager/Options/Create
        public IActionResult Create(int? QuestionId)
        {
            ViewData["qst"] = QuestionId;
            ViewData["QuestionId"] = new SelectList(_context.Questions.Where(i => i.QuestionId == QuestionId), "QuestionId", "QName");
            return View();
        }

        // POST: Manager/Options/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OptionId,OptName,Correct,QuestionId")] Option option)
        {
            if (ModelState.IsValid)
            {
                _context.Add(option);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Questions", new { id = option.QuestionId });
            }
            ViewData["qst"] = option.QuestionId;
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QName", option.QuestionId);
            return View(option);
        }

        // GET: Manager/Options/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var option = await _context.Options.FindAsync(id);
            ViewData["qst"] = option.QuestionId;
            if (option == null)
            {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QName", option.QuestionId);
            return View(option);
        }

        // POST: Manager/Options/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OptionId,OptName,Correct,QuestionId")] Option option)
        {
            if (id != option.OptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(option);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionExists(option.OptionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Questions", new { id = option.QuestionId });
            }
            ViewData["qst"] = option.QuestionId;
            ViewData["QuestionId"] = new SelectList(_context.Questions, "QuestionId", "QName", option.QuestionId);
            return View(option);
        }

        // GET: Manager/Options/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Options
                .Include(o => o.Question)
                .FirstOrDefaultAsync(m => m.OptionId == id);
            if (option == null)
            {
                return NotFound();
            }

            return View(option);
        }

        // POST: Manager/Options/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var option = await _context.Options.FindAsync(id);
            _context.Options.Remove(option);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Questions", new { id = option.QuestionId });
        }

        private bool OptionExists(int id)
        {
            return _context.Options.Any(e => e.OptionId == id);
        }
    }
}
