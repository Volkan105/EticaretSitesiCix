using EticaretSitesiCix.Data;
using EticaretSitesiCix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Diger.Rol_Admin)]
    public class KullaniciController : Controller
    {
        private readonly ApplicationDbContext _context;
        public KullaniciController(ApplicationDbContext context)
        {
            _context = context;
        }
      
        public IActionResult Index()
        {
            var kullanicilar = _context.Kullanıcılar.ToList();
            var roller = _context.Roles.ToList();
            var kullaniciRol = _context.UserRoles.ToList();
            foreach (var item in kullanicilar)
            {
                var roleId = kullaniciRol.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Rol = roller.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return View(kullanicilar);
        }
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kullanici = await _context.Kullanıcılar
                .FirstOrDefaultAsync(m => m.Id == id.ToString());
            if (kullanici == null)
            {
                return NotFound();
            }

            return View(kullanici);
        }

        // POST: Admin/Categorie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var kullanici = await _context.Kullanıcılar.FindAsync(id);
            _context.Kullanıcılar.Remove(kullanici);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}

