using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_ship
	{
		public int api_member_id { get; set; }
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public string api_name { get; set; }
		public string api_yomi { get; set; }
		public int api_stype { get; set; }
		public int api_ship_id { get; set; }
		public int api_lv { get; set; }
		public int api_exp { get; set; }
		public int api_afterlv { get; set; }
		public int api_aftershipid { get; set; }
		public int api_nowhp { get; set; }
		public int api_maxhp { get; set; }
		public int[] api_taik { get; set; }
		public int[] api_souk { get; set; }
		public int[] api_houg { get; set; }
		public int[] api_raig { get; set; }
		public int[] api_baku { get; set; }
		public int[] api_tyku { get; set; }
		public int[] api_houm { get; set; }
		public int[] api_raim { get; set; }
		public int[] api_saku { get; set; }
		public int[] api_luck { get; set; }
		public int api_soku { get; set; }
		public int api_leng { get; set; }
		public int[] api_slot { get; set; }
		public int[] api_onslot { get; set; }
		public int[] api_broken { get; set; }
		public int[] api_powup { get; set; }
		public int[] api_kyouka { get; set; }
		public int api_backs { get; set; }
		public int api_fuel { get; set; }
		public int api_fuel_max { get; set; }
		public int api_bull { get; set; }
		public int api_bull_max { get; set; }
		public object api_gomes { get; set; }
		public object api_gomes2 { get; set; }
		public int api_slotnum { get; set; }
		public int api_ndock_time { get; set; }
		public string api_ndock_time_str { get; set; }
		public int[] api_ndock_item { get; set; }
		public int api_star { get; set; }
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
		public int api_use_fuel { get; set; }
		public int api_use_bull { get; set; }
		public int api_voicef { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
