using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
    // ReSharper disable InconsistentNaming
    public class kcsapi_battleresult
    {
        public string api_win_rank { get; set; }
        public string api_quest_name { get; set; }
        public int api_quest_level { get; set; }
        public kcsapi_enemyinfo api_enemy_info { get; set; }
        public kcsapi_getship api_get_ship { get; set; }
    }

    public class kcsapi_getship
    {
        public int api_ship_id { get; set; }
        public string api_ship_name { get; set; }
    }

    public class kcsapi_enemyinfo
    {
        public string api_deck_name { get; set; }
    }
    // ReSharper restore InconsistentNaming
}