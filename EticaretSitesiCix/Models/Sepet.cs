using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class Sepet
    {
        public Sepet()
        {
            UrunSayisi = 1;
        }
        [Key]
        public int Id { get; set; }
        public string KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; }
        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; }
        public int UrunSayisi { get; set; }
        [NotMapped]
        public double Fiyat { get; set; }
    }
}
