using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_remodel_slotlist
	{
		public int api_id { get; set; }
		public int api_slot_id { get; set; }
		public int api_req_fuel { get; set; }
		public int api_req_bull { get; set; }
		public int api_req_steel { get; set; }
		public int api_req_bauxite { get; set; }
		public int api_req_buildkit { get; set; }
		public int api_req_remodelkit { get; set; }
		public int api_req_slot_id { get; set; }
		public int api_req_slot_num { get; set; }
	}
}