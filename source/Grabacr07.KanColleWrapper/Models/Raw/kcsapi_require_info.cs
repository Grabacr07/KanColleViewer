using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_require_info
	{
		public kcsapi_basic api_basic { get; set; }
		public kcsapi_slotitem[] api_slot_item { get; set; }
		//public Api_Unsetslot api_unsetslot { get; set; }
		public kcsapi_kdock[] api_kdock { get; set; }
		public kcsapi_useitem[] api_useitem { get; set; }
		//public Api_Furniture[] api_furniture { get; set; }
	}
}
