using EticaretSitesiCix.Data;
using EticaretSitesiCix.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Search(string q)
        {
            if(!String.IsNullOrEmpty(q))
            {
                var ara = _db.Urunler.Where(i => i.Adi.Contains(q) || i.Aciklama.Contains(q));
                return View(ara);
            }
            return View();
        }
        public IActionResult CategoryDetails(int? id)
        {
            var product = _db.Urunler.Where(i => i.CategoryId == id).ToList();
            ViewBag.KategoriId = id;
            return View(product);
        }

        public IActionResult Index()
        {
            var urunler = _db.Urunler.Where(i=>i.AnaSayfadaMi).ToList();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var UrunSayisi = _db.Sepetler.Where(i => i.KullaniciId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Diger.SessionSepet, UrunSayisi);
            }
            return View(urunler);
        }
        public IActionResult Details(int id)
        {
            var urunler = _db.Urunler.FirstOrDefault(m => m.Id == id);
            Sepet sepetim = new Sepet()
            {
                Urun = urunler,
                UrunId = urunler.Id
            };
            return View(sepetim);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Sepet spt)
        {
            spt.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                spt.KullaniciId = claim.Value;
                Sepet cart = _db.Sepetler.FirstOrDefault(m => m.KullaniciId == spt.KullaniciId && m.UrunId == spt.UrunId);
                if(cart==null)
                {
                    _db.Sepetler.Add(spt);
                }
                else
                {
                    cart.UrunSayisi += spt.UrunSayisi;
                }
                _db.SaveChanges();
                var sayi = _db.Sepetler.Where(i => i.KullaniciId == spt.KullaniciId).ToList().Count();
                HttpContext.Session.SetInt32(Diger.SessionSepet, sayi);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var urunler = _db.Urunler.FirstOrDefault(m => m.Id == spt.Id);
                Sepet sepetim = new Sepet()
                {
                    Urun = urunler,
                    UrunId = urunler.Id
                };
            }
            
            return View(spt);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
