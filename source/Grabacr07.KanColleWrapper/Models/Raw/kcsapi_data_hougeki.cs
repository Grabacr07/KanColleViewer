using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_data_hougeki
    {
        public int[] api_at_list { get; set; }
        public int[] api_at_type { get; set; }
        public object[] api_df_list { get; set; }
        public object[] api_si_list { get; set; }
        public object[] api_cl_list { get; set; }
        public object[] api_damage { get; set; }
    }
    public class kcsapi_data_midnight_hougeki
    {
        public int[] api_at_list { get; set; }
        public object[] api_df_list { get; set; }
        public object[] api_si_list { get; set; }
        public object[] api_cl_list { get; set; }
        public int[] api_sp_list { get; set; }
        public object[] api_damage { get; set; }
    }
}
