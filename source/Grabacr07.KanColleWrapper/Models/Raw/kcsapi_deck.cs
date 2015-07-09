using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_deck
	{
		public int api_member_id { get; set; }
		public int api_id { get; set; }
		public string api_name { get; set; }
		public string api_name_id { get; set; }
		public long[] api_mission { get; set; }
		public string api_flagship { get; set; }
		public int[] api_ship { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
