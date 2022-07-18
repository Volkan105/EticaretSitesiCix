using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class SiparisDetay
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int SiparisId { get; set; }
        [ForeignKey("SiparisId")]
        public Siparisler Siparis { get; set; }
        public int UrunId { get; set; }
        [ForeignKey("UrunId")]
        public Urun Urun { get; set; }
        public int UrunAdedi { get; set; }
        public double Fiyat { get; set; }
    }
}
