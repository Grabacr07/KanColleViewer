using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_data_raigeki
    {
        public int[] api_frai { get; set; }
        public int[] api_erai { get; set; }
        public decimal[] api_fdam { get; set; }
        public decimal[] api_edam { get; set; }
        public decimal[] api_fydam { get; set; }
        public decimal[] api_eydam { get; set; }
        public int[] api_fcl { get; set; }
        public int[] api_ecl { get; set; }
    }
}
