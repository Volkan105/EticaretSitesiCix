using EticaretSitesiCix.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EticaretSitesiCix.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kullanici> Kullanıcılar { get; set; }
        public DbSet<Siparisler> Siparisler { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<SiparisDetay> SiparisDetaylar { get; set; }
    }
}
