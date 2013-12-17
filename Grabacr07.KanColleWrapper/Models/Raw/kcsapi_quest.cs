using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_quest
	{
		public int api_no { get; set; }
		public int api_category { get; set; }
		public int api_type { get; set; }
		public int api_state { get; set; }
		public string api_title { get; set; }
		public string api_detail { get; set; }
		public int[] api_get_material { get; set; }
		public int api_bonus_flag { get; set; }
		public int api_progress_flag { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
