using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    public class kcsapi_practice_result
    {
        public int[] api_ship_id { get; set; }
        public string api_win_rank { get; set; }
        public int api_get_exp { get; set; }
        public int api_member_lv { get; set; }
        public int api_member_exp { get; set; }
        public int api_get_base_exp { get; set; }
        public int api_mvp { get; set; }
        public int[] api_get_ship_exp { get; set; }
        public int[][] api_get_exp_lvup { get; set; }
        public int api_dests { get; set; }
        public int api_destsf { get; set; }
        public kcsapi_data_enemyinfo api_enemy_info { get; set; }
    }
}
