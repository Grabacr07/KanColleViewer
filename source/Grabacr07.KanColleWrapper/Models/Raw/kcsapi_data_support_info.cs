using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_data_support_info
    {
        public kcsapi_data_support_airattack api_support_airatack { get; set; }
        public kcsapi_data_support_hourai api_support_hourai { get; set; }
    }
    public class kcsapi_data_support_airattack
    {
        public int api_deck_id { get; set; }
        public int[] api_ship_id { get; set; }
        public int[] api_undressing_flag { get; set; }
        public int[] api_stage_flag { get; set; }
        public int[][] api_plane_from { get; set; }
        public kcsapi_data_stage1 api_stage1 { get; set; }
        public kcsapi_data_stage2 api_stage2 { get; set; }
        public kcsapi_data_stage3 api_stage3 { get; set; }
    }
    public class kcsapi_data_support_hourai
    {
        public int api_deck_id { get; set; }
        public int[] api_ship_id { get; set; }
        public int[] api_undressing_flag { get; set; }
        public int[] api_cl_list { get; set; }
        public decimal[] api_damage { get; set; }
    }
}
