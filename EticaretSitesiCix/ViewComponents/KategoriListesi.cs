using EticaretSitesiCix.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.ViewComponents
{
    public class KategoriListesi:ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public KategoriListesi(ApplicationDbContext db)
        {
            _db = db;
        }
          public IViewComponentResult Invoke()
        {
            var category = _db.Categories.ToList();
            return View(category);
        }
    }
}
