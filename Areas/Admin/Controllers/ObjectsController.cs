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

namespace KRU.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ObjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Objects
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Objects.Include(o => o.Address);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Objects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objects = await _context.Objects
                .Include(o => o.Address)
                .FirstOrDefaultAsync(m => m.ObjectId == id);
            if (objects == null)
            {
                return NotFound();
            }

            return View(objects);
        }

        // GET: Admin/Objects/Create
        public IActionResult Create()
        {
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building");
            return View();
        }

        // POST: Admin/Objects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjectId,ObjectName,AddressId")] Objects objects)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objects);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", objects.AddressId);
            return View(objects);
        }

        // GET: Admin/Objects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objects = await _context.Objects.FindAsync(id);
            if (objects == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", objects.AddressId);
            return View(objects);
        }

        // POST: Admin/Objects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjectId,ObjectName,AddressId")] Objects objects)
        {
            if (id != objects.ObjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objects);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjectsExists(objects.ObjectId))
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
            ViewData["AddressId"] = new SelectList(_context.Addresses, "AddressId", "Building", objects.AddressId);
            return View(objects);
        }

        // GET: Admin/Objects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objects = await _context.Objects
                .Include(o => o.Address)
                .FirstOrDefaultAsync(m => m.ObjectId == id);
            if (objects == null)
            {
                return NotFound();
            }

            return View(objects);
        }

        // POST: Admin/Objects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objects = await _context.Objects.FindAsync(id);
            _context.Objects.Remove(objects);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObjectsExists(int id)
        {
            return _context.Objects.Any(e => e.ObjectId == id);
        }
    }
}
