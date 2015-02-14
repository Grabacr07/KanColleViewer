using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_remodel_slotlist_detail
	{
		public int api_req_buildkit { get; set; }
		public int api_req_remodelkit { get; set; }
		public int api_certain_buildkit { get; set; }
		public int api_certain_remodelkit { get; set; }
		public int api_req_slot_id { get; set; }
		public int api_req_slot_num { get; set; }
		public int api_change_flag { get; set; }
	}
}