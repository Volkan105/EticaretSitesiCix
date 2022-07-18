using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class Urun
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Adi { get; set; }
        public string Aciklama { get; set; }
        public int Olcu { get; set; }
        public string Model { get; set; }
        public double Fiyat { get; set; }
        public string Fotograf { get; set; }
        public bool AnaSayfadaMi { get; set; }
        public bool StoktaMi { get; set; }        
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

    }
}
