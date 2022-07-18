using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EticaretSitesiCix.Models
{
    public class SepetimViewModel
    {
        public IEnumerable<Sepet> ListCart { get; set; }
        public Siparisler OrderHeader { get; set; }
    }
}
