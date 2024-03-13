using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoeBay.Data;
using ShoeBay.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ShoeBay.Controllers
{
    public class ShoesController : Controller
    {
        private readonly ShoeBayDBContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ShoesController(ShoeBayDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;   
        }

        // GET: Shoes
        public async Task<IActionResult> Index()
        {
              return _context.Shoes != null ? 
                          View(await _context.Shoes.ToListAsync()) :
                          Problem("Entity set 'ShoeBayDBContext.Shoes'  is null.");
        }

        // GET: Shoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shoes == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoe == null)
            {
                return NotFound();
            }

            return View(shoe);
        }

        // GET: Shoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Shoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Brand,Description,Size,Color,Cost,Images")] Shoe shoe)
        {
            
                string uniqueFileName = UploadedFile(shoe);
                shoe.FileUrl = uniqueFileName;
                _context.Add(shoe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
            return View(shoe);
        }
        private string UploadedFile(Shoe shoes)
        {
            string uniqueFileName = null;
            if (shoes.Images != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + shoes.Images.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    shoes.Images.CopyTo(fileStream);
                }
            }
            return uniqueFileName;

        }
        // GET: Shoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shoes == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe == null)
            {
                return NotFound();
            }
            return View(shoe);
        }

        // POST: Shoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Brand,Description,Size,Color,Cost")] Shoe shoe)
        {
            if (id != shoe.Id)
            {
                return NotFound();
            }

           // if (ModelState.IsValid)
            {
                try
                {
                    string uniqueFileName = UploadedFile(shoe);
                    shoe.FileUrl = uniqueFileName;
                    _context.Update(shoe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoeExists(shoe.Id))
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
            return View(shoe);
        }

        // GET: Shoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shoes == null)
            {
                return NotFound();
            }

            var shoe = await _context.Shoes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shoe == null)
            {
                return NotFound();
            }

            return View(shoe);
        }

        // POST: Shoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shoes == null)
            {
                return Problem("Entity set 'ShoeBayDBContext.Shoes'  is null.");
            }
            var shoe = await _context.Shoes.FindAsync(id);
            if (shoe != null)
            {
                _context.Shoes.Remove(shoe);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoeExists(int id)
        {
          return (_context.Shoes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
