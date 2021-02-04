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

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Manager/Tasks
        public async Task<IActionResult> Index()
        {
            //dynamic model = new ExpandoObject();
            //model.Task_File = _context.Task_Files.Include(t => t.FileHistory);
            var applicationDbContext = _context.Tasks.Include(t => t.Department).Include(t => t.Task_Type).Include(t => t.Task_Files).ThenInclude(t => t.FileHistory);
                return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Department)
                .Include(t => t.Task_Type)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // GET: Manager/Tasks/Create
        public IActionResult Create()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.FileHistory.Include(w => w.Task_Files).ThenInclude(w => w.Tasks).Where(w => w.FileFinished == false))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.Name + " #" + item.FileUrl,
                    Value = item.FileId.ToString(),

                };
                SelectListItem sel = selectListItem;
                items.Add(sel);
            }
            ViewBag.File = items;
            //ViewBag
            //ViewData["FileType"] = _context.Task_Files.Include(t => t.FileHistory).ToList();
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType");
            return View();
        }

        // POST: Manager/Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaskId,SumLost,SumGain,Comment,File,Finished,TaskStarted,TaskEnd,DepartmentId,TaskTypeId")] Tasks tasks, [Bind("Task_FileId,TaskId,FileId")] Task_File task_file)
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            tasks.DepartmentId = DepId;
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                
                task_file.TaskId = tasks.TaskId;
                //task_file.Task_FileId = FileId;
                _context.Add(task_file);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }
        //CREATE FILE TASKS

        [HttpPost]
        public JsonResult CreateFiles(List<int> ListofFiles, int TaskId)
        {
            return Json(data:"", new Newtonsoft.Json.JsonSerializerSettings());
        }
        //
        // GET: Manager/Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "TaskTypeID", tasks.TaskTypeId);
            return View(tasks);
        }

        // POST: Manager/Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,SumLost,SumGain,Comment,File,Finished,TaskStarted,TaskEnd,DepartmentId,TaskTypeId")] Tasks tasks)
        {
            if (id != tasks.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tasks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TasksExists(tasks.TaskId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "TaskTypeID", tasks.TaskTypeId);
            return View(tasks);
        }

        // GET: Manager/Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks
                .Include(t => t.Department)
                .Include(t => t.Task_Type)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Manager/Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TasksExists(int id)
        {
            return _context.Tasks.Any(e => e.TaskId == id);
        }
    }
}
