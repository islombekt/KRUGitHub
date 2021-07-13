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
using Microsoft.AspNetCore.Http;
using System.IO;

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
            var DepId = _context.Managers.Include(t => t.User).ThenInclude(d => d.Department).ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).User.Department.DepartmentId;

            var applicationDbContext = _context.Tasks.Include(t => t.Department).Include(t => t.Task_Type).Include(t => t.Task_Files).ThenInclude(t => t.FileHistory).Include(i => i.Task_Emples).ThenInclude(u => u.Employee)
                .ThenInclude(u => u.User).Where(r => r.DepartmentId == DepId).OrderByDescending(t => t.TaskId);
            return View(await applicationDbContext.ToListAsync());
        }
      
        // GET: Manager/Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tasks = await _context.Tasks.Include(u => u.Employee).ThenInclude(u => u.User).Include(u => u.Task_Emples).ThenInclude(t => t.Employee).ThenInclude(u => u.User)
                .Include(t => t.Department)
                .Include(t => t.Task_Type)
                .FirstOrDefaultAsync(m => m.TaskId == id);
            if (tasks == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial", tasks);
        }

        // GET: Manager/Tasks/Create
        public IActionResult Create()
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;

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

            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;

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
        public async Task<IActionResult> Create([Bind("TaskId,TaskName,SumLost,SumGain,Comment,File,Finished,TaskStarted,TaskEnd,DepartmentId,TaskTypeId")] Tasks tasks, [Bind("Task_FileId,TaskId,FileId")] Task_File task_file, [Bind("Task_EmpId", "TaskId", "EmplId")] Task_Empl task_emp,List<int> ListofFiles, List<int> ListofEmp)
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
                foreach (var i in ListofEmp)
                {
                    Task_Empl eT = new Task_Empl()
                    {
                        TaskId = tasks.TaskId,
                        EmplId = i
                    };
                    _context.Task_Empls.Add(eT);
                    await _context.SaveChangesAsync();
                }

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

            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
           ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }
        // GET: Manager/Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;

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
            var enm = _context.Task_Empls.Where(i => i.TaskId == id).ToList().Select(i => i.EmplId);
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

            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;


            List<SelectListItem> MasulEmpl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                MasulEmpl.Add(sel);
            }
            ViewBag.MasulEmp = MasulEmpl;



            tasks.selectedFiles = new List<int> { };
            foreach (int i in a.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedFiles.Add(i);
                }
            }

            tasks.selectedEmployees = new List<int> { };
            foreach(int i in enm.ToArray())
            {
                if(i != 0)
                {
                    tasks.selectedEmployees.Add(i);
                }
            }
            ViewBag.FileSelected = _context.FileHistory.Include(t => t.Task_Files).ThenInclude( i=> i.Tasks).ToList().Select(i => i.Name);
            ViewBag.EmployeeSelected = _context.Employees.Include(u => u.User).Include(t => t.Task_Emples).ThenInclude(i => i.Tasks).ToList().Select(i => i.User.FullName);
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", tasks.DepartmentId);
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", tasks.TaskTypeId);
            return View(tasks);
        }

        // POST: Manager/Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("TaskId,TaskName,SumLost,SumGain,Comment,File,Finished,TaskStarted,AllEmpl,TaskEnd,DepartmentId,TaskTypeId,MasulEmplId")] Tasks tasks, List<int> ListofFiles, List<int> ListofEmpl, IFormFile file)
        {
            string prevFile = (await _context.Tasks.AsNoTracking().ToListAsync()).FirstOrDefault(i=>i.TaskId==tasks.TaskId).File;
            if (prevFile != null && file != null)
            {
                if (System.IO.File.Exists(Path.Combine("wwwroot/files/end", prevFile)))
                {
                    // If file found, delete it    
                    System.IO.File.Delete(Path.Combine("wwwroot/files/end", prevFile));
                }
            }
            var ManId = _context.Managers.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            if (file != null)
            {
                string type = Path.GetExtension(file.FileName);
                if ((type != ".docx") && (type != ".doc") && (type != ".pdf") && (type != ".xlsm") && (type != ".xlsx"))
                    return Content("Нотоғри файл тури танланди");
                //return View("~/Views/Shared/_UnsupportedMediatype.cshtml");
                tasks.File = file.FileName;
            }
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

                    var task_emp = _context.Task_Empls.Where(w => w.TaskId == id).ToList();
                    if(task_emp != null)
                    {
                        foreach(var i in task_emp)
                        {
                            _context.Task_Empls.Remove(i);
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

                    foreach(var it in ListofEmpl)
                    {
                        Task_Empl enT = new Task_Empl()
                        {
                            TaskId = tasks.TaskId,
                            EmplId = it,
                        };
                        _context.Task_Empls.Add(enT);
                        await _context.SaveChangesAsync();
                    }

                    
                    if (file != null)
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/end", file.FileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
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
            ViewBag.File = items;

            var a = _context.Task_Files.Where(i => i.TaskId == id).ToList().Select(i => i.FileId);
            var enm = _context.Task_Empls.Where(i => i.TaskId == id).ToList().Select(i => i.EmplId);

            tasks.selectedFiles = new List<int> { };
            foreach (int i in a.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedFiles.Add(i);
                }
            }
            List<SelectListItem> empl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                empl.Add(sel);
            }
            ViewBag.Emp = empl;

            List<SelectListItem> MasulEmpl = new List<SelectListItem>();
            foreach (var item in _context.Employees.Include(u => u.User).Where(w => w.ManagerId == ManId && w.EmployeeState != "out"))
            {
                SelectListItem selectListItem = new SelectListItem
                {
                    Text = item.User.FullName,
                    Value = item.EmployeeId.ToString(),

                };
                SelectListItem sel = selectListItem;
                MasulEmpl.Add(sel);
            }
            ViewBag.MasulEmp = MasulEmpl;

            tasks.selectedEmployees = new List<int> { };
            foreach (int i in enm.ToArray())
            {
                if (i != 0)
                {
                    tasks.selectedEmployees.Add(i);
                }
            }
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
            var taskss = await _context.Tasks.Include(u => u.Task_Emples).ThenInclude(t => t.Employee).ThenInclude(u => u.User)
              .Include(t => t.Department)
              .Include(t => t.Task_Type)
              .FirstOrDefaultAsync(m => m.TaskId == id);
            var fin = await _context.FinanceReports.Where(u => u.TaskId == id).ToListAsync();
            int b = 0;
            if (fin != null)
            {
                b = fin.Count();
            }
            var rep = await _context.Reports.Where(r => r.TaskId == id).ToListAsync();
            int a = 0;
            if(rep != null)
            {
                a = rep.Count();
            }
            Tasks tasks = new Tasks
            {
                TaskId = taskss.TaskId,
                TaskName = taskss.TaskName,
                TaskStarted = taskss.TaskStarted,
                TaskEnd = taskss.TaskEnd,
                SumLost = taskss.SumLost,
                SumGain = taskss.SumGain,
                Comment = taskss.Comment,
                Department = taskss.Department,
                Reports = taskss.Reports,
                Finished = taskss.Finished,
                File = taskss.File,
                MasulEmplId = taskss.MasulEmplId,
                FinanceReports=fin,
                FinCount = b,
                RepCount = a,
                Task_Emples = taskss.Task_Emples
                

            };
          
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

            var task_empl = _context.Task_Empls.Where(w => w.TaskId == id).ToList();
            if(task_empl != null)
            {
                foreach(var i in task_empl)
                {
                    _context.Task_Empls.Remove(i);
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
