using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_mission
	{
		public int api_id { get; set; }
		public int api_maparea_id { get; set; }
		public string api_name { get; set; }
		public string api_details { get; set; }
		public int api_time { get; set; }
		public int api_difficulty { get; set; }
		public float api_use_fuel { get; set; }
		public float api_use_bull { get; set; }
		public int[] api_win_item1 { get; set; }
		public int[] api_win_item2 { get; set; }
	}
	// ReSharper restore InconsistentNaming
}