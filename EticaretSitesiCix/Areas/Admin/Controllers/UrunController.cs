using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EticaretSitesiCix.Data;
using EticaretSitesiCix.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace EticaretSitesiCix.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =Diger.Rol_Admin)]
    public class UrunController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;

        public UrunController(ApplicationDbContext context,IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }

        // GET: Admin/Urun
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Urunler.Include(u => u.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Urun/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler
                .Include(u => u.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // GET: Admin/Urun/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }
         
        // POST: Admin/Urun/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Urun urun)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count>0)
                {
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(_he.WebRootPath, @"~/images/ÜrünResimleri/");
                    var ext = Path.GetExtension(files[0].FileName);
                    if(urun.Fotograf!=null)
                    {
                        var imagepath = Path.Combine(_he.WebRootPath, urun.Fotograf.TrimStart('\\'));
                        if(System.IO.File.Exists(imagepath))
                        {
                            System.IO.File.Delete(imagepath);
                        }
                    }
                    using(var filestreams=new FileStream(Path.Combine(upload,filename +ext),FileMode.Create))
                    {
                        files[0].CopyTo(filestreams);
                    }
                    urun.Fotograf = @"images\ÜrünResimleri" + filename + ext;
                }
                _context.Add(urun);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           
            return View(urun);
        }

        // GET: Admin/Urun/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler.FindAsync(id);
            if (urun == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", urun.CategoryId);
            return View(urun);
        }

        // POST: Admin/Urun/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Urun urun)
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                string filename = Guid.NewGuid().ToString();
                var upload = Path.Combine(_he.WebRootPath, @"images\ÜrünResimleri");
                var ext = Path.GetExtension(files[0].FileName);
                if (urun.Fotograf != null)
                {
                    var imagepath = Path.Combine(_he.WebRootPath, urun.Fotograf.TrimStart('\\'));
                    if (System.IO.File.Exists(imagepath))
                    {
                        System.IO.File.Delete(imagepath);
                    }
                }
                using (var filestreams = new FileStream(Path.Combine(upload, filename + ext), FileMode.Create))
                {
                    files[0].CopyTo(filestreams);
                }
                urun.Fotograf = @"images\ÜrünResimleri" + filename + ext;
            }

            if (ModelState.IsValid)
            {
              
                    _context.Update(urun);
                    await _context.SaveChangesAsync();
                
               
                return RedirectToAction(nameof(Index));
            }
            
            return View(urun);
        }

        // GET: Admin/Urun/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var urun = await _context.Urunler
                .Include(u => u.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (urun == null)
            {
                return NotFound();
            }

            return View(urun);
        }

        // POST: Admin/Urun/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var urun = await _context.Urunler.FindAsync(id);
            var imagepath = Path.Combine(_he.WebRootPath, urun.Fotograf.TrimStart('\\'));
            if (System.IO.File.Exists(imagepath))
            {
                System.IO.File.Delete(imagepath);
            }
            _context.Urunler.Remove(urun);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UrunExists(int id)
        {
            return _context.Urunler.Any(e => e.Id == id);
        }
    }
}
