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
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace KRU.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class FinanceReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public FinanceReportsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpGet]
        public async Task<JsonResult> GetCat2(int? catId1)
        {
            var cat2 = await _context.Category11s.Where(i => i.Category1Id == catId1).ToListAsync();

            return Json(cat2);
        }
        [HttpGet]
        public async Task<JsonResult> GetCat3(int? catId2)
        {
            var cat2 = await _context.Category111s.Where(i => i.Category11Id == catId2).ToListAsync();

            return Json(cat2);
        }



        [HttpGet]
        public async Task<JsonResult> GetObject(int? addressId)
        {
            var objectss = await _context.Objects.Where(i => i.AddressId == addressId).ToListAsync();
            return Json(objectss);
        }
        [HttpPost]
        public async Task<IActionResult> PostFinance([FromBody] finModel Object)
        {

            //dynamic finance  = JsonConvert.DeserializeObject(finObj.ToString());
            var finance = Object;
            int addId;
            int catid;
            var d = finance.GetType();
           return RedirectToAction(nameof(Index));
           
        }
        // GET: Employee/FinanceReports
        public async Task<IActionResult> Index()
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var applicationDbContext = _context.FinanceReports.Include(f => f.Address).Include(f => f.Category1)
                .Include(f => f.Category11).Include(f => f.Category111).Include(f => f.Employee)
                .Include(f => f.Manager).Include(f => f.Objects).Include(f => f.Tasks).Where(r => r.EmployeeId == EmpId).OrderByDescending(u => u.FinanceRepId);
            return View(await applicationDbContext.ToListAsync());
        }

       

        // GET: Employee/FinanceReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financeReport = await _context.FinanceReports
                .Include(f => f.Address)
                .Include(f => f.Category1)
                .Include(f => f.Category11)
                .Include(f => f.Category111)
                .Include(f => f.Employee).ThenInclude(u => u.User)
                .Include(f => f.Manager).ThenInclude(u => u.User)
                .Include(f => f.Objects)
                .Include(f => f.Tasks)
                .FirstOrDefaultAsync(m => m.FinanceRepId == id);
            if (financeReport == null)
            {
                return NotFound();
            }

            return PartialView("DetailsPartial",financeReport);
        }

        [HttpPost]
        public IActionResult CreateAfterCat(int FinanceRepId, DateTime StartDate,string Status, DateTime EndDate, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId,bool HaveSeen, string CheckPeriod)
        {

            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            
            

            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;
            //ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111");

            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                Status = Status,
                CheckPeriod = CheckPeriod
            };
            return View("Create", finance);
        }
        [HttpPost]
        public IActionResult CreateAfterCat1(int FinanceRepId, DateTime StartDate, string Status, DateTime EndDate,bool HaveSeen, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");

            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                Status = Status,
                CheckPeriod = CheckPeriod
            };
            return View("Create", finance);
        }

        [HttpPost]
        public IActionResult CreateAfterAddress(int FinanceRepId, DateTime StartDate,bool HaveSeen, string Status, DateTime EndDate, double amount,string FinComment,int TaskId, int Category1Id, int Category11Id,int Category111Id,int AddressId, int ObjectId, string CheckPeriod)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");


            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.ObjectName,
                    Value = item.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                items.Add(s);
            }
            ViewData["ObjectId"] = items;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                Status = Status,
                CheckPeriod = CheckPeriod
            };
            return View("Create", finance);
        }
        // GET: Employee/FinanceReports/Create
        public IActionResult Create()
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var PrevRep = _context.FinanceReports.ToList().OrderByDescending(r => r.FinanceRepId).FirstOrDefault(r => r.EmployeeId == EmpId);

            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11");
            ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId");
            List<SelectListItem> itemsObj = new List<SelectListItem>();
            if(PrevRep != null)
            {

            
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == PrevRep.AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
          
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;

            if (PrevRep == null)
            {
                return View();
            }
            return View(PrevRep);
        }

        // POST: Employee/FinanceReports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FinanceRepId,StartDate,EndDate,amount,Status,FinComment,TaskId,Category1Id,Category11Id,Category111Id,AddressId,ObjectId,ManagerId,EmployeeId,HaveSeen,CheckPeriod")] FinanceReport financeReport)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;
            financeReport.EmployeeId = EmpId;
            financeReport.ManagerId = ManId;
            financeReport.HaveSeen = false;
            Report report = new Report
            {
                ManagerId = ManId,
                EmployeeId = EmpId,
                ReportDate = DateTime.Now,
                State = financeReport.Status,
                HaveSeen = false,
                TaskId = financeReport.TaskId,
                AddressId = financeReport.AddressId,
                ObjectId = financeReport.ObjectId,
                ReportDescription = "Ҳисоботнинг таҳлилий маълумотномаси толдирилди",
            };
            if (ModelState.IsValid)
            {
                _context.Add(financeReport);
                await _context.SaveChangesAsync();
                _context.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", financeReport.AddressId);
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", financeReport.Category1Id);
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", financeReport.Category11Id);
            ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111", financeReport.Category111Id);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", financeReport.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", financeReport.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectName", financeReport.ObjectId);
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View(financeReport);
        }

        [HttpPost]
        public IActionResult EditAfterCat(int FinanceRepId, DateTime StartDate, string Status, DateTime EndDate,bool HaveSeen, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId,string CheckPeriod)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");

           
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;


            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;
            //ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111");

            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                EmployeeId = EmpId,
                ManagerId = ManId,
                HaveSeen= HaveSeen,
                CheckPeriod = CheckPeriod,
                Status = Status
            };
            return View("Edit", finance);
        }
        [HttpPost]
        public IActionResult EditAfterCat1(int FinanceRepId, DateTime StartDate, string Status, DateTime EndDate, bool HaveSeen,double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11");
            
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                ObjectId = ObjectId,
                EmployeeId = EmpId,
                ManagerId = ManId,
                HaveSeen = HaveSeen,
                CheckPeriod = CheckPeriod,
                Status = Status
            };
            return View("Edit", finance);
        }

        [HttpPost]
        public IActionResult EditAfterAddress(int FinanceRepId, DateTime StartDate, string Status, DateTime EndDate,bool HaveSeen, double amount, string FinComment, int TaskId, int Category1Id, int Category11Id, int Category111Id, int AddressId, int ObjectId, string CheckPeriod)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1");
           
            var ManId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).ManagerId;


            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;


            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == Category11Id && i.Category11.Category1Id == Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> itemsObj = new List<SelectListItem>();
            foreach (var itemObj in _context.Objects.Where(i => i.AddressId == AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = itemObj.ObjectName,
                    Value = itemObj.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                itemsObj.Add(s);
            }
            ViewData["ObjectId"] = itemsObj;
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            FinanceReport finance = new FinanceReport
            {
                FinanceRepId = FinanceRepId,
                StartDate = StartDate,
                EndDate = EndDate,
                amount = amount,
                FinComment = FinComment,
                TaskId = TaskId,
                Category1Id = Category1Id,
                Category11Id = Category11Id,
                Category111Id = Category111Id,
                AddressId = AddressId,
                EmployeeId = EmpId,
                ManagerId = ManId,
                ObjectId = ObjectId,
                HaveSeen = HaveSeen,
                CheckPeriod = CheckPeriod,
                Status = Status
            };
            return View("Edit", finance);
        }

        // GET: Employee/FinanceReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            if (id == null)
            {
                return NotFound();
            }

            var financeReport = await _context.FinanceReports.FindAsync(id);
            if (financeReport == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", financeReport.AddressId);
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", financeReport.Category1Id);

            List<SelectListItem> itemsC1 = new List<SelectListItem>();
            foreach (var item1 in _context.Category11s.Where(i => i.Category1Id == financeReport.Category1Id))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item1.Name_C11,
                    Value = item1.Category11Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC1.Add(s);
            }
            ViewData["Category11Id"] = itemsC1;
            List<SelectListItem> itemsC11 = new List<SelectListItem>();
            foreach (var item11 in _context.Category111s.Include(c => c.Category11).Where(i => (i.Category11Id == financeReport.Category11Id && i.Category11.Category1Id == financeReport.Category1Id)))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item11.Name_C111,
                    Value = item11.Category111Id.ToString(),
                };
                SelectListItem s = sel;
                itemsC11.Add(s);
            }
            ViewData["Category111Id"] = itemsC11;


            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var item in _context.Objects.Where(i => i.AddressId == financeReport.AddressId))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.ObjectName,
                    Value = item.ObjectId.ToString(),
                };
                SelectListItem s = sel;
                items.Add(s);
            }
            ViewData["ObjectId"] = items;



            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View(financeReport);
        }
     
        // POST: Employee/FinanceReports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int FinanceRepId, [Bind("FinanceRepId,StartDate,EndDate,Status,amount,FinComment,TaskId,Category1Id,Category11Id,Category111Id,AddressId,ObjectId,ManagerId,EmployeeId,HaveSeen,CheckPeriod")] FinanceReport financeReport)
        {
            var EmpId = _context.Employees.ToList().FirstOrDefault(u => u.UserId == _userManager.GetUserId(User)).EmployeeId;
            if (FinanceRepId != financeReport.FinanceRepId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(financeReport);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinanceReportExists(financeReport.FinanceRepId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", financeReport.AddressId);
            ViewData["Category1Id"] = new SelectList(_context.Category1s, "Category1Id", "Name_C1", financeReport.Category1Id);
            ViewData["Category11Id"] = new SelectList(_context.Category11s, "Category11Id", "Name_C11", financeReport.Category11Id);
            ViewData["Category111Id"] = new SelectList(_context.Category111s, "Category111Id", "Name_C111", financeReport.Category111Id);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", financeReport.EmployeeId);
            ViewData["ManagerId"] = new SelectList(_context.Managers, "ManagerId", "ManagerId", financeReport.ManagerId);
            ViewData["ObjectId"] = new SelectList(_context.Objects, "ObjectId", "ObjectName", financeReport.ObjectId);
            var DepId = _context.User.ToList().FirstOrDefault(u => u.Id == _userManager.GetUserId(User)).DepartmentId;
            List<SelectListItem> tasks = new List<SelectListItem>();
            foreach (var item in _context.Tasks.Where(i => i.DepartmentId == DepId && i.Finished == false && i.Finished == false && (i.Task_Emples.ToList().Select(ir => ir.TaskId).Count() == 0 || i.Task_Emples.ToList().Select(ir => ir.EmplId).Contains(EmpId))))
            {
                SelectListItem sel = new SelectListItem
                {
                    Text = item.TaskName,
                    Value = item.TaskId.ToString(),
                };
                SelectListItem s = sel;
                tasks.Add(s);
            }
            ViewData["TaskId"] = tasks;
            return View(financeReport);
        }

        // GET: Employee/FinanceReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financeReport = await _context.FinanceReports
                .Include(f => f.Address)
                .Include(f => f.Category1)
                .Include(f => f.Category11)
                .Include(f => f.Category111)
                .Include(f => f.Employee).ThenInclude(u => u.User)
                .Include(f => f.Manager).ThenInclude(u => u.User)
                .Include(f => f.Objects)
                .Include(f => f.Tasks)
                .FirstOrDefaultAsync(m => m.FinanceRepId == id);
            if (financeReport == null)
            {
                return NotFound();
            }

            return View(financeReport);
        }

        // POST: Employee/FinanceReports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var financeReport = await _context.FinanceReports.FindAsync(id);
            _context.FinanceReports.Remove(financeReport);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinanceReportExists(int id)
        {
            return _context.FinanceReports.Any(e => e.FinanceRepId == id);
        }
    }
}
