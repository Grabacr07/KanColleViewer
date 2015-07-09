using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_port
	{
		public kcsapi_material[] api_material { get; set; }
		public kcsapi_deck[] api_deck_port { get; set; }
		public kcsapi_ndock[] api_ndock { get; set; }
		public kcsapi_ship2[] api_ship { get; set; }
		public kcsapi_basic api_basic { get; set; }
		public int api_combined_flag { get; set; }
		//public Api_Log[] api_log { get; set; }
	}

	//public class Api_Basic
	//{
	//	public string api_member_id { get; set; }
	//	public string api_nickname { get; set; }
	//	public string api_nickname_id { get; set; }
	//	public int api_active_flag { get; set; }
	//	public long api_starttime { get; set; }
	//	public int api_level { get; set; }
	//	public int api_rank { get; set; }
	//	public int api_experience { get; set; }
	//	public object api_fleetname { get; set; }
	//	public string api_comment { get; set; }
	//	public string api_comment_id { get; set; }
	//	public int api_max_chara { get; set; }
	//	public int api_max_slotitem { get; set; }
	//	public int api_max_kagu { get; set; }
	//	public int api_playtime { get; set; }
	//	public int api_tutorial { get; set; }
	//	public int[] api_furniture { get; set; }
	//	public int api_count_deck { get; set; }
	//	public int api_count_kdock { get; set; }
	//	public int api_count_ndock { get; set; }
	//	public int api_fcoin { get; set; }
	//	public int api_st_win { get; set; }
	//	public int api_st_lose { get; set; }
	//	public int api_ms_count { get; set; }
	//	public int api_ms_success { get; set; }
	//	public int api_pt_win { get; set; }
	//	public int api_pt_lose { get; set; }
	//	public int api_pt_challenged { get; set; }
	//	public int api_pt_challenged_win { get; set; }
	//	public int api_firstflag { get; set; }
	//	public int api_tutorial_progress { get; set; }
	//	public int[] api_pvp { get; set; }
	//	public int api_large_dock { get; set; }
	//}

	//public class Api_Material
	//{
	//	public int api_member_id { get; set; }
	//	public int api_id { get; set; }
	//	public int api_value { get; set; }
	//}

	//public class Api_Deck_Port
	//{
	//	public int api_member_id { get; set; }
	//	public int api_id { get; set; }
	//	public string api_name { get; set; }
	//	public string api_name_id { get; set; }
	//	public int[] api_mission { get; set; }
	//	public string api_flagship { get; set; }
	//	public int[] api_ship { get; set; }
	//}

	//public class Api_Ndock
	//{
	//	public int api_member_id { get; set; }
	//	public int api_id { get; set; }
	//	public int api_state { get; set; }
	//	public int api_ship_id { get; set; }
	//	public int api_complete_time { get; set; }
	//	public string api_complete_time_str { get; set; }
	//	public int api_item1 { get; set; }
	//	public int api_item2 { get; set; }
	//	public int api_item3 { get; set; }
	//	public int api_item4 { get; set; }
	//}

	//public class Api_Ship
	//{
	//	public int api_id { get; set; }
	//	public int api_sortno { get; set; }
	//	public int api_ship_id { get; set; }
	//	public int api_lv { get; set; }
	//	public int[] api_exp { get; set; }
	//	public int api_nowhp { get; set; }
	//	public int api_maxhp { get; set; }
	//	public int api_leng { get; set; }
	//	public int[] api_slot { get; set; }
	//	public int[] api_onslot { get; set; }
	//	public int[] api_kyouka { get; set; }
	//	public int api_backs { get; set; }
	//	public int api_fuel { get; set; }
	//	public int api_bull { get; set; }
	//	public int api_slotnum { get; set; }
	//	public int api_ndock_time { get; set; }
	//	public int[] api_ndock_item { get; set; }
	//	public int api_srate { get; set; }
	//	public int api_cond { get; set; }
	//	public int[] api_karyoku { get; set; }
	//	public int[] api_raisou { get; set; }
	//	public int[] api_taiku { get; set; }
	//	public int[] api_soukou { get; set; }
	//	public int[] api_kaihi { get; set; }
	//	public int[] api_taisen { get; set; }
	//	public int[] api_sakuteki { get; set; }
	//	public int[] api_lucky { get; set; }
	//	public int api_locked { get; set; }
	//}

	//public class Api_Log
	//{
	//	public int api_no { get; set; }
	//	public string api_type { get; set; }
	//	public string api_state { get; set; }
	//	public string api_message { get; set; }
	//}

	// ReSharper restore InconsistentNaming
}
