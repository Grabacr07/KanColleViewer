using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_combined_battle_airbattle
	{
		public int api_deck_id { get; set; }
		public int[] api_ship_ke { get; set; }
		public int[] api_ship_lv { get; set; }
		public int[] api_nowhps { get; set; }
		public int[] api_maxhps { get; set; }
		public int[] api_nowhps_combined { get; set; }
		public int[] api_maxhps_combined { get; set; }
		public int api_midnight_flag { get; set; }
		public int[][] api_eSlot { get; set; }
		public int[][] api_eKyouka { get; set; }
		public int[][] api_fParam { get; set; }
		public int[][] api_eParam { get; set; }
		public int[][] api_fParam_combined { get; set; }
		public int[] api_search { get; set; }
		public int[] api_formation { get; set; }
		public kcsapi_data_airbase_injection api_air_base_injection { get; set; }
		public kcsapi_data_kouku api_injection_kouku { get; set; }
		public kcsapi_data_airbaseattack[] api_air_base_attack { get; set; }
		public int[] api_stage_flag { get; set; }
		public kcsapi_data_kouku api_kouku { get; set; }
		public int api_support_flag { get; set; }
		public kcsapi_data_support_info api_support_info { get; set; }
		public int[] api_stage_flag2 { get; set; }
		public kcsapi_data_kouku api_kouku2 { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
