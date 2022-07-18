using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class Kullanici:IdentityUser
    {
        [Required]
        public string Ad { get; set; }
        [Required]
        public string Soyad { get; set; }
        public string Adres { get; set; }
        public string Sehir { get; set; }
        public string Semt { get; set; }
        public string PostaKodu { get; set; }
        [NotMapped]
        public string Rol { get; set; }
    }
}
