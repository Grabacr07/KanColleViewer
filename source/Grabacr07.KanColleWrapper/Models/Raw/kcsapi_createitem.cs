using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_createitem
	{
		public int api_create_flag { get; set; }
		public kcsapi_slotitem[] api_get_items { get; set; }
		public int[] api_material { get; set; }
		public kcsapi_unsetitems[] kcsapi_unset_items { get; set; }

		public class kcsapi_unsetitems
        {
			public int api_type3 { get; set; }
			public int api_slot_list { get; set; }
        }
		//public int api_id { get; set; }
		//public int api_slotitem_id { get; set; }
		//public int api_shizai_flag { get; set; }
		//public string api_fdata { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
