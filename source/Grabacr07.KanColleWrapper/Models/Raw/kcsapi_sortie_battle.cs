using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_sortie_battle
    {
        public int api_dock_id { get; set; }
        public int[] api_ship_ke { get; set; }
        public int[] api_ship_lv { get; set; }
        public int[] api_nowhps { get; set; }
        public int[] api_maxhps { get; set; }
        public int api_midnight_flag { get; set; }
        public int[][] api_eSlot { get; set; }
        public int[][] api_eKyouka { get; set; }
        public int[][] api_fParam { get; set; }
        public int[][] api_eParam { get; set; }
        public int[] api_search { get; set; }
        public int[] api_formation { get; set; }
        public kcsapi_data_airbaseattack[] api_air_base_attack { get; set; }
        public int[] api_stage_flag { get; set; }
        public kcsapi_data_kouku api_kouku { get; set; }
        public int api_support_flag { get; set; }
        public kcsapi_data_support_info api_support_info { get; set; }
        public int api_opening_taisen_flag { get; set; }
        public kcsapi_data_hougeki api_opening_taisen { get; set; }
        public int api_opening_flag { get; set; }
        public kcsapi_data_raigeki api_opening_atack { get; set; }
        public int[] api_hourai_flag { get; set; }
        public kcsapi_data_hougeki api_hougeki1 { get; set; }
        public kcsapi_data_hougeki api_hougeki2 { get; set; }
        public kcsapi_data_hougeki api_hougeki3 { get; set; }
        public kcsapi_data_raigeki api_raigeki { get; set; }
    }
}
