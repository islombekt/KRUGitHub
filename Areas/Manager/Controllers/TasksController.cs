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
        public async Task<IActionResult> Create([Bind("TaskId,SumLost,SumGain,Comment,File,Finished,TaskStarted,TaskEnd,DepartmentId,TaskTypeId")] Tasks tasks, [Bind("Task_FileId,TaskId,FileId")] Task_File task_file, List<int> ListofFiles)
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            tasks.DepartmentId = DepId;
            if (ModelState.IsValid)
            {
                _context.Add(tasks);
                await _context.SaveChangesAsync();
                foreach(var item in ListofFiles)
                {
                    Task_File ttt = new Task_File()
                    {
                        TaskId = tasks.TaskId,
                        FileId = item
                };

                    _context.Task_Files.Add(ttt);
                    await _context.SaveChangesAsync();

                }
                //task_file.TaskId = tasks.TaskId;
                //task_file.Task_FileId = FileId;
                //_context.Add(task_file);
               await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
           ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }
        //CREATE FILE TASKS

        //[HttpPost]
        //public IActionResult CreLocChanger(int TaskId,string SumLost,string SumGain,string Comment,string File,bool Finished,DateTime TaskStarted, DateTime TaskEnd,int DepartmentId,int TaskTypeId)
        //{

        //    List<SelectListItem> items = new List<SelectListItem>();
        //    foreach (var item in _context.FileHistory.Include(w => w.Task_Files).ThenInclude(w => w.Tasks).Where(w => w.FileFinished == false))
        //    {
        //        SelectListItem selectListItem = new SelectListItem
        //        {
        //            Text = item.Name + " #" + item.FileUrl,
        //            Value = item.FileId.ToString(),

        //        };
        //        SelectListItem sel = selectListItem;
        //        items.Add(sel);
        //    }
        //    ViewBag.File = items;
        //    Tasks task = new Tasks { TaskId = TaskId,SumLost=SumLost, SumGain =SumGain, Comment=Comment, File=File, Finished=Finished, TaskStarted=TaskStarted, TaskEnd=TaskEnd, DepartmentId=DepartmentId,TaskTypeId=TaskTypeId};
        //    ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", task.TaskTypeId);
        //    return View("Create",task);
        //}
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
            
            var a = _context.Task_Files.Where(i => i.TaskId == id).ToList().Select(i => i.FileId);
            
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
            //foreach (var i in a) {
            //    tasks.selectedFiles.Add(i.GetValueOrDefault());
            //}
            tasks.selectedFiles = new List<int> { };
            foreach (int i in a.ToArray())
            {
                if (i != null)
                {
                    tasks.selectedFiles.Add(i);
                }
            }

            //tasks.Task_Files = (ICollection<Task_File>)a;
            //tasks.Task_Files.Add(a);
            //tasks.selectedFiles = a.ToArray(); 
            ViewBag.FileSelected = _context.FileHistory.Include(t => t.Task_Files).ThenInclude( i=> i.Tasks).ToList().Select(i => i.Name);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }

        // POST: Manager/Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TaskId,SumLost,SumGain,Comment,File,Finished,TaskStarted,TaskEnd,DepartmentId,TaskTypeId")] Tasks tasks, List<int> ListofFiles)
        {
            if (id != tasks.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var task_file = _context.Task_Files.Where(w => w.TaskId == id).ToList();
                    if (task_file != null)
                    {
                        foreach (var i in task_file)
                        {
                            _context.Task_Files.Remove(i);
                        }
                    }
                    foreach (var item in ListofFiles)
                    {
                        Task_File ttt = new Task_File()
                        {
                            TaskId = tasks.TaskId,
                            FileId = item
                        };

                        _context.Task_Files.Add(ttt);
                        await _context.SaveChangesAsync();

                    }

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
            var a = _context.Task_Files.Where(i => i.TaskId == id).ToList().Select(i => i.FileId);
            tasks.selectedFiles = new List<int> { };
            foreach (int i in a.ToArray())
            {
                if (i != null)
                {
                    tasks.selectedFiles.Add(i);
                }
            }
            ViewBag.File = items;

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
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
            var task_file = _context.Task_Files.Where(w => w.TaskId == id).ToList();
            if(task_file != null)
            {
                foreach(var i in task_file)
                {
                    _context.Task_Files.Remove(i);
                }
            }
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
