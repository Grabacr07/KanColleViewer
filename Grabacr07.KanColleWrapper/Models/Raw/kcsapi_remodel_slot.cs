using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	class kcsapi_remodel_slot
	{
		public int[] api_after_material { get; set; }
		public AfterSlot api_after_slot { get; set; }
		public int api_remodel_flag;
		public int[] api_remodel_id;
		public int api_voice_id;

	}
	class AfterSlot
	{
		public int api_id;
		public int api_level;
		public int api_locked;
		public int api_slotitem_id;
	}
}
