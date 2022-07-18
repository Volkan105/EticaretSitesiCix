using EticaretSitesiCix.Data;
using EticaretSitesiCix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SiparisController : Controller
    {
      
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public SiparisDetayViewModel SiparisViewModel { get; set; }
        public SiparisController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpPost]
        [Authorize(Roles =Diger.Rol_Admin)]
        public IActionResult Onaylandi()
        {
            Siparisler siparisler = _db.Siparisler.FirstOrDefault(i => i.Id == SiparisViewModel.Siparis.Id);
            siparisler.SiparisDurumu = Diger.DurumOnaylandı;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = Diger.Rol_Admin)]
        public IActionResult KargoyaVer()
        {
            Siparisler siparisler = _db.Siparisler.FirstOrDefault(i => i.Id == SiparisViewModel.Siparis.Id);
            siparisler.SiparisDurumu = Diger.DurumKargoda;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            
            SiparisViewModel = new SiparisDetayViewModel
            {
                Siparis = _db.Siparisler.FirstOrDefault(i => i.Id == id),
                SiparisDetay = _db.SiparisDetaylar.Where(x => x.SiparisId == id).Include(x => x.Urun)
            };
            return View(SiparisViewModel);
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Siparisler> SiparisListesi;
            if(User.IsInRole(Diger.Rol_Admin))
            {
                SiparisListesi = _db.Siparisler.ToList();
            }
            else
            {
                SiparisListesi = _db.Siparisler.Where(i => i.KullaniciId == claim.Value).Include(i=>i.Kullanici);
            }
            return View(SiparisListesi);
        }
        public IActionResult Beklenen()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Siparisler> SiparisListesi;
            if (User.IsInRole(Diger.Rol_Admin))
            {
                SiparisListesi = _db.Siparisler.Where(i => i.SiparisDurumu == Diger.DurumBeklemede);
            }
            else
            {
                SiparisListesi = _db.Siparisler.Where(i => i.KullaniciId == claim.Value&&i.SiparisDurumu==Diger.DurumBeklemede).Include(i => i.Kullanici);
            }
            return View(SiparisListesi);
        }
        public IActionResult Onaylanan()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Siparisler> SiparisListesi;
            if (User.IsInRole(Diger.Rol_Admin))
            {
                SiparisListesi = _db.Siparisler.Where(i => i.SiparisDurumu == Diger.DurumOnaylandı);
            }
            else
            {
                SiparisListesi = _db.Siparisler.Where(i => i.KullaniciId == claim.Value && i.SiparisDurumu == Diger.DurumOnaylandı).Include(i => i.Kullanici);
            }
            return View(SiparisListesi);
        }
        public IActionResult Kargoda()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Siparisler> SiparisListesi;
            if (User.IsInRole(Diger.Rol_Admin))
            {
                SiparisListesi = _db.Siparisler.Where(i => i.SiparisDurumu == Diger.DurumKargoda);
            }
            else
            {
                SiparisListesi = _db.Siparisler.Where(i => i.KullaniciId == claim.Value && i.SiparisDurumu == Diger.DurumKargoda).Include(i => i.Kullanici);
            }
            return View(SiparisListesi);
        }
    }
}
