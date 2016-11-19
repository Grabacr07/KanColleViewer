using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_mst_mapinfo
	{
		public int api_id { get; set; }
		public int api_maparea_id { get; set; }
		public int api_no { get; set; }
		public string api_name { get; set; }
		public int api_level { get; set; }
		public string api_opetext { get; set; }
		public string api_infotext { get; set; }
		public int[] api_item { get; set; }
		public int? api_max_maphp { get; set; }
		public int? api_required_defeat_count { get; set; }
		public int[] api_sally_flag { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
