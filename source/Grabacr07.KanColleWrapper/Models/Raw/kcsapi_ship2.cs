using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_ship2
	{
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public int api_ship_id { get; set; }
		public int api_lv { get; set; }
		public int[] api_exp { get; set; }
		public int api_nowhp { get; set; }
		public int api_maxhp { get; set; }
		public int api_leng { get; set; }
		public int[] api_slot { get; set; }
		public int[] api_onslot { get; set; }
		public int[] api_kyouka { get; set; }
		public int api_backs { get; set; }
		public int api_fuel { get; set; }
		public int api_bull { get; set; }
		public int api_slotnum { get; set; }
		public long api_ndock_time { get; set; }
		public int[] api_ndock_item { get; set; }
		public int api_srate { get; set; }
		public int api_cond { get; set; }
		public int[] api_karyoku { get; set; }
		public int[] api_raisou { get; set; }
		public int[] api_taiku { get; set; }
		public int[] api_soukou { get; set; }
		public int[] api_kaihi { get; set; }
		public int[] api_taisen { get; set; }
		public int[] api_sakuteki { get; set; }
		public int[] api_lucky { get; set; }
		public int api_locked { get; set; }
		public int api_sally_area { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
