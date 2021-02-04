using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KRU.Data;
using KRU.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace KRU.Areas.Manager.Controllers
{
    [Area("Manager")]
    [Authorize(Roles = SD.Role_Manager)]
    public class FileHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Manager/FileHistories
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.FileHistory.Include(f => f.Task_Type);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Manager/FileHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileHistory = await _context.FileHistory
                .Include(f => f.Task_Type)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (fileHistory == null)
            {
                return NotFound();
            }

            return View(fileHistory);
        }

        // GET: Manager/FileHistories/Create
        public IActionResult Create()
        {
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types,"TaskTypeID", "NameType");
            return View();
        }

        // POST: Manager/FileHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, [Bind("FileId,Name,FileUrl,Description,FileStart,FileEnd,FileFinished,TaskTypeId")] FileHistory fileHistory)
        {
            fileHistory.FileFinished = false;
            //if (file == null || file.Length == 0)
            //    return Content("Файл танланмади");
            if (file != null)
            {
                string type = Path.GetExtension(file.FileName);
                if ((type != ".docx") && (type != ".doc") && (type != ".pdf"))
                    return Content("Нотогри файл тури танланди");
                //return View("~/Views/Shared/_UnsupportedMediatype.cshtml");
                fileHistory.FileUrl = file.FileName;
            }
            if (ModelState.IsValid)
            {
                _context.Add(fileHistory);
                await _context.SaveChangesAsync();
                if (file != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "NameType", fileHistory.TaskTypeId);
            return View(fileHistory);
        }

        // GET: Manager/FileHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileHistory = await _context.FileHistory.FindAsync(id);
            if (fileHistory == null)
            {
                return NotFound();
            }
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "TaskTypeID", fileHistory.TaskTypeId);
            return View(fileHistory);
        }

        // POST: Manager/FileHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FileId,Name,FileUrl,Description,FileStart,FileEnd,FileFinished,TaskTypeId")] FileHistory fileHistory)
        {
            if (id != fileHistory.FileId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fileHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FileHistoryExists(fileHistory.FileId))
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
            ViewData["TaskTypeId"] = new SelectList(_context.Task_Types, "TaskTypeID", "TaskTypeID", fileHistory.TaskTypeId);
            return View(fileHistory);
        }

        // GET: Manager/FileHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fileHistory = await _context.FileHistory
                .Include(f => f.Task_Type)
                .FirstOrDefaultAsync(m => m.FileId == id);
            if (fileHistory == null)
            {
                return NotFound();
            }

            return View(fileHistory);
        }

        // POST: Manager/FileHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fileHistory = await _context.FileHistory.FindAsync(id);
            _context.FileHistory.Remove(fileHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FileHistoryExists(int id)
        {
            return _context.FileHistory.Any(e => e.FileId == id);
        }




        //File upload Download
      
        [HttpGet]
        public async Task<IActionResult> Download_Config(int FileId)
        {
            if (FileId == null)
                return Content("Файл мавжуд эмас");
            var file = await _context.FileHistory.FirstOrDefaultAsync(w => w.FileId == FileId);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files", file.FileUrl);
            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, GetContentType(path), Path.GetFileName(path));
            }
            catch
            {
                return View("~/Views/Shared/_NotFound.cshtml", file.FileUrl);
            }

            return Ok();

        }
        private string GetContentType(string path)
        {

            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }
        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain" },
                {".pdf", "application/pdf" },
                {".doc", "application/vnd.ms-word" },
                {".docx", "application/vnd.ms-word" }
            };
        }


    }
}
