using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_combined_battle_battleresult : kcsapi_battleresult
	{
		public int api_mvp_combined { get; set; }
		public int[] api_get_ship_exp_combined { get; set; }
		public int[][] api_get_exp_lvup_combined { get; set; }
		public int api_escape_flag { get; set; }
		public Api_Escape api_escape { get; set; }
	}

	public class Api_Enemy_Info
	{
		public string api_level { get; set; }
		public string api_rank { get; set; }
		public string api_deck_name { get; set; }
	}

	public class Api_Get_Ship
	{
		public int api_ship_id { get; set; }
		public string api_ship_type { get; set; }
		public string api_ship_name { get; set; }
		public string api_ship_getmes { get; set; }
	}

	public class Api_Escape
	{
		public int[] api_escape_idx { get; set; }
		public int[] api_tow_idx { get; set; }
	}

	// ReSharper restore InconsistentNaming
}
