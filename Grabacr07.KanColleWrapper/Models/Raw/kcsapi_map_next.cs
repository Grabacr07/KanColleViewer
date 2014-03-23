using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_map_next
	{
		public int api_rashin_flg { get; set; }
		public int api_rashin_id { get; set; }
		public int api_maparea_id { get; set; }
		public int api_mapinfo_no { get; set; }
		public int api_no { get; set; }
		public int api_color_no { get; set; }
		public int api_event_id { get; set; }
		public int api_next { get; set; }
		public int api_bosscell_no { get; set; }
		public int api_bosscomp { get; set; }

		public api_map_enemy api_enemy { get; set; }
		public api_map_itemget api_itemget { get; set; }
	}

	public class api_map_enemy
	{
		public int api_enemy_id { get; set; }
		public int api_result { get; set; }
		public string api_result_str { get; set; }
	}

	public class api_map_itemget
	{
		public int api_usemst { get; set; }
		public int api_id { get; set; }
		public int api_getcount { get; set; }
		public string api_name { get; set; }
		public int api_icon_id { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
