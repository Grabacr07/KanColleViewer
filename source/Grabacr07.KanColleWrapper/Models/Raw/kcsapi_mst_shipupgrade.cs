using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_mst_shipupgrade
	{
		public int api_id { get; set; }

		public int api_current_ship_id { get; set; }
		public int api_original_ship_id { get; set; }

		public int api_upgrade_type { get; set; }
		public int api_upgrade_level { get; set; }

		public int api_drawing_count { get; set; }
		public int api_catapult_count { get; set; }

		public int api_sortno { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
