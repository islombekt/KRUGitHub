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
    public class Task_TypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Task_TypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager/Task_Type
        public async Task<IActionResult> Index()
        {
            return View(await _context.Task_Types.ToListAsync());
        }

        // GET: Manager/Task_Type/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task_Type = await _context.Task_Types
                .FirstOrDefaultAsync(m => m.TaskTypeID == id);
            if (task_Type == null)
            {
                return NotFound();
            }

            return View(task_Type);
        }

        // GET: Manager/Task_Type/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Manager/Task_Type/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskTypeID,NameType")] Task_Type task_Type)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task_Type);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task_Type);
        }

        // GET: Manager/Task_Type/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task_Type = await _context.Task_Types.FindAsync(id);
            if (task_Type == null)
            {
                return NotFound();
            }
            return View(task_Type);
        }

        // POST: Manager/Task_Type/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskTypeID,NameType")] Task_Type task_Type)
        {
            if (id != task_Type.TaskTypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task_Type);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Task_TypeExists(task_Type.TaskTypeID))
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
            return View(task_Type);
        }

        // GET: Manager/Task_Type/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task_Type = await _context.Task_Types
                .FirstOrDefaultAsync(m => m.TaskTypeID == id);
            if (task_Type == null)
            {
                return NotFound();
            }

            return View(task_Type);
        }

        // POST: Manager/Task_Type/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task_Type = await _context.Task_Types.FindAsync(id);
            _context.Task_Types.Remove(task_Type);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Task_TypeExists(int id)
        {
            return _context.Task_Types.Any(e => e.TaskTypeID == id);
        }
    }
}
