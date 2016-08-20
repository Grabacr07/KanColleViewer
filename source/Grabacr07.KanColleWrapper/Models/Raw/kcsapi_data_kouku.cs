using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_data_kouku
    {
        public int[][] api_plane_from { get; set; }
        public kcsapi_data_stage1 api_stage1 { get; set; }
        public kcsapi_data_stage2 api_stage2 { get; set; }
        public kcsapi_data_stage3 api_stage3 { get; set; }
        public kcsapi_data_stage3_combined api_stage3_combined { get; set; }
    }

    public class kcsapi_data_stage1
    {
        public int api_f_count { get; set; }
        public int api_f_lostcount { get; set; }
        public int api_e_count { get; set; }
        public int api_e_lostcount { get; set; }
        public int api_disp_seiku { get; set; }
        public int[] api_touch_plane { get; set; }
    }
    public class kcsapi_data_stage2
    {
        public int api_f_count { get; set; }
        public int api_f_lostcount { get; set; }
        public int api_e_count { get; set; }
        public int api_e_lostcount { get; set; }
        public kcsapi_data_airfire api_air_fire { get; set; }
    }
    public class kcsapi_data_airfire
    {
        public int api_idx { get; set; }
        public int api_kind { get; set; }
        public int[] api_use_items { get; set; }
    }
    public class kcsapi_data_stage3
    {
        public int[] api_frai_flag { get; set; }
        public int[] api_erai_flag { get; set; }
        public int[] api_fbak_flag { get; set; }
        public int[] api_ebak_flag { get; set; }
        public int[] api_fcl_flag { get; set; }
        public int[] api_ecl_flag { get; set; }
        public decimal[] api_fdam { get; set; }
        public decimal[] api_edam { get; set; }
    }
    public class kcsapi_data_stage3_combined
    {
        public int[] api_frai_flag { get; set; }
        public int[] api_fbak_flag { get; set; }
        public int[] api_fcl_flag { get; set; }
        public decimal[] api_fdam { get; set; }
    }
}
