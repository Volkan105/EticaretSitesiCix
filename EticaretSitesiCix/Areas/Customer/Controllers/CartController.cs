using EticaretSitesiCix.Data;
using EticaretSitesiCix.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public SepetimViewModel SepetimViewModel { get; set; }
        public CartController(UserManager<IdentityUser> userManager,
             ApplicationDbContext db)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            SepetimViewModel = new SepetimViewModel()
            {
                OrderHeader = new Models.Siparisler(),
                ListCart = _db.Sepetler.Where(i => i.KullaniciId == claim.Value).Include(i => i.Urun)
            };
            foreach (var item in SepetimViewModel.ListCart)
            {
                item.Fiyat = item.Urun.Fiyat;
                SepetimViewModel.OrderHeader.SiparisTutari += (item.UrunSayisi * item.Urun.Fiyat);
            }
            return View(SepetimViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(SepetimViewModel model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            SepetimViewModel.ListCart = _db.Sepetler.Where(i => i.KullaniciId == claim.Value).Include(i => i.Urun);
            SepetimViewModel.OrderHeader.SiparisDurumu = Diger.DurumBeklemede;
            SepetimViewModel.OrderHeader.KullaniciId = claim.Value;
            SepetimViewModel.OrderHeader.SiparisTarihi = DateTime.Now;
            _db.Siparisler.Add(SepetimViewModel.OrderHeader);
            _db.SaveChanges();
            foreach (var item in SepetimViewModel.ListCart)
            {
                item.Fiyat = item.Urun.Fiyat;
                SiparisDetay siparisDetay = new SiparisDetay()
                {
                    UrunId = item.UrunId,
                    SiparisId = SepetimViewModel.OrderHeader.Id,
                    Fiyat = item.Fiyat,
                    UrunAdedi = item.UrunSayisi
                };
                SepetimViewModel.OrderHeader.SiparisTutari += item.Urun.Fiyat;
                model.OrderHeader.SiparisTutari += item.Urun.Fiyat;
                _db.SiparisDetaylar.Add(siparisDetay);
            }

            var payment = PaymentProcess(model);
            _db.Sepetler.RemoveRange(SepetimViewModel.ListCart);
            _db.SaveChanges();
            HttpContext.Session.SetInt32(Diger.SessionSepet, 0);
            return RedirectToAction("SiparisTamam");
        }

        private Payment PaymentProcess(SepetimViewModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-VJdIdmz1GahlvlnoOQLI3keA5UxH3e4Z";
            options.SecretKey = "sandbox-aDIjSQJfp3SNG6LDdHqgc3vmpBg5fycf";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = new Random().Next(1111, 9999).ToString();
            request.Price = model.OrderHeader.SiparisTutari.ToString();
            request.PaidPrice = model.OrderHeader.SiparisTutari.ToString();
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = "B67832";
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();
            /*
            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = "John Doe";
            paymentCard.CardNumber = "5528790000000008";
            paymentCard.ExpireMonth = "12";
            paymentCard.ExpireYear = "2030";
            paymentCard.Cvc = "123";
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;
            */
            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.OrderHeader.KartAdi;
            paymentCard.CardNumber = model.OrderHeader.KartNumarası;
            paymentCard.ExpireMonth = model.OrderHeader.SonKullanmaAy;
            paymentCard.ExpireYear = model.OrderHeader.SonKullanmaYil;
            paymentCard.Cvc = model.OrderHeader.Cvc;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer();
            buyer.Id = model.OrderHeader.Id.ToString();
            buyer.Name = model.OrderHeader.Ad;
            buyer.Surname = model.OrderHeader.Soyad;
            buyer.GsmNumber = model.OrderHeader.TelefonNo;
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = model.OrderHeader.Adres;
            buyer.Ip = "85.34.78.112";
            buyer.City = model.OrderHeader.Sehir;
            buyer.Country = "Turkey";
            buyer.ZipCode = model.OrderHeader.PostaKodu;
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            
            request.BasketItems = basketItems;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            foreach (var item in _db.Sepetler.Where(i=>i.KullaniciId==claim.Value).Include(i=>i.Urun))
            {
                basketItems.Add(new BasketItem()
                {
                    Id = item.Id.ToString(),
                    Name = item.Urun.Adi,
                    Category1 = item.Urun.CategoryId.ToString(),
                    ItemType=BasketItemType.PHYSICAL.ToString(),
                    Price=(item.Fiyat*item.UrunSayisi).ToString()
                });

            }
            return Payment.Create(request, options);
        }

        public IActionResult SiparisTamam()
        {
            return View();
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            SepetimViewModel = new SepetimViewModel()
            {
                OrderHeader = new Models.Siparisler(),
                ListCart = _db.Sepetler.Where(i => i.KullaniciId == claim.Value).Include(i => i.Urun)
            };
            SepetimViewModel.OrderHeader.SiparisTutari = 0;
            SepetimViewModel.OrderHeader.Kullanici = _db.Kullanıcılar.FirstOrDefault(i => i.Id == claim.Value);
            foreach (var item in SepetimViewModel.ListCart)
            {
                SepetimViewModel.OrderHeader.SiparisTutari += (item.UrunSayisi * item.Urun.Fiyat);
            }
            return View(SepetimViewModel);
        }
       
        public IActionResult Add(int sepetId)
        {
            var cart = _db.Sepetler.Where(i=>i.Id==sepetId).FirstOrDefault();
            cart.UrunSayisi += 1;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Decrease(int sepetId)
        {
            var cart = _db.Sepetler.Where(i => i.Id == sepetId).FirstOrDefault();
            if(cart.UrunSayisi==1)
            {
                var count = _db.Sepetler.Where(u => u.KullaniciId == cart.KullaniciId).ToList().Count();
                _db.Sepetler.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.SessionSepet, count - 1);
            }
            else
            {
                cart.UrunSayisi -= 1;
                _db.SaveChanges();
            }
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int sepetId)
        {
            var cart = _db.Sepetler.Where(i => i.Id == sepetId).FirstOrDefault();
           
                var count = _db.Sepetler.Where(u => u.KullaniciId == cart.KullaniciId).ToList().Count();
                _db.Sepetler.Remove(cart);
                _db.SaveChanges();
                HttpContext.Session.SetInt32(Diger.SessionSepet, count - 1);
           
            return RedirectToAction(nameof(Index));
        }

    }
}
