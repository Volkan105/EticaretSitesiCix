using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class SiparisDetayViewModel
    {
        public Siparisler Siparis { get; set; }
        public IEnumerable<SiparisDetay> SiparisDetay { get; set; }
    }
}
