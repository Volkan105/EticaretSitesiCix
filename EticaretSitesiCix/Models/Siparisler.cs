using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class Siparisler
    {
        [Key]
        public int Id { get; set; }
        public string KullaniciId { get; set; }
        [ForeignKey("KullaniciId")]
        public Kullanici Kullanici { get; set; }
        public DateTime SiparisTarihi { get; set; }
        public double SiparisTutari { get; set; }
        public string SiparisDurumu { get; set; }
        [Required]
        public string Ad { get; set; }
        [Required]
        public string Soyad { get; set; }
        [Required]
        public string TelefonNo { get; set; }
        [Required]
        public string Adres { get; set; }
        [Required]
        public string Semt { get; set; }
        [Required]
        public string Sehir { get; set; }
        [Required]
        public string PostaKodu { get; set; }
        [Required]
        public string KartAdi { get; set; }
        [Required]
        public string KartNumarası { get; set; }
        [Required]
        public string SonKullanmaAy { get; set; }
        [Required]
        public string SonKullanmaYil { get; set; }
        [Required]
        public string Cvc { get; set; }
    }
}
