using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_ndock
	{
		public int api_member_id { get; set; }
		public int api_id { get; set; }
		public int api_state { get; set; }
		public int api_ship_id { get; set; }
		public long api_complete_time { get; set; }
		public string api_complete_time_str { get; set; }
		public int api_item1 { get; set; }
		public int api_item2 { get; set; }
		public int api_item3 { get; set; }
		public int api_item4 { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
