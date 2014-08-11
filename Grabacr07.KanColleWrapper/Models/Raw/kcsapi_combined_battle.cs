using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_combined_battle
	{
		public string api_deck_id { get; set; }
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
		public object[] api_formation { get; set; }
		public int[] api_stage_flag { get; set; }
		public Api_Kouku api_kouku { get; set; }
		public int api_support_flag { get; set; }
		public object api_support_info { get; set; }
		public int api_opening_flag { get; set; }
		public object api_opening_atack { get; set; }
		public int[] api_hourai_flag { get; set; }
		public Api_Hougeki1 api_hougeki1 { get; set; }
		public Api_Raigeki api_raigeki { get; set; }
		public Api_Hougeki2 api_hougeki2 { get; set; }
		public Api_Hougeki3 api_hougeki3 { get; set; }
	}
	
	public class Api_Kouku
	{
		public int[][] api_plane_from { get; set; }
		public Api_Stage1 api_stage1 { get; set; }
		public Api_Stage2 api_stage2 { get; set; }
		public Api_Stage3 api_stage3 { get; set; }
		public Api_Stage3_Combined api_stage3_combined { get; set; }
	}

	public class Api_Stage1
	{
		public int api_f_count { get; set; }
		public int api_f_lostcount { get; set; }
		public int api_e_count { get; set; }
		public int api_e_lostcount { get; set; }
		public int api_disp_seiku { get; set; }
		public int[] api_touch_plane { get; set; }
	}

	public class Api_Stage2
	{
		public int api_f_count { get; set; }
		public int api_f_lostcount { get; set; }
		public int api_e_count { get; set; }
		public int api_e_lostcount { get; set; }
	}

	public class Api_Stage3
	{
		public int[] api_frai_flag { get; set; }
		public int[] api_erai_flag { get; set; }
		public int[] api_fbak_flag { get; set; }
		public int[] api_ebak_flag { get; set; }
		public int[] api_fcl_flag { get; set; }
		public int[] api_ecl_flag { get; set; }
		public int[] api_fdam { get; set; }
		public int[] api_edam { get; set; }
	}

	public class Api_Stage3_Combined
	{
		public int[] api_frai_flag { get; set; }
		public int[] api_fbak_flag { get; set; }
		public int[] api_fcl_flag { get; set; }
		public float[] api_fdam { get; set; }
	}

	public class Api_Hougeki1
	{
		public int[] api_at_list { get; set; }
		public int[] api_at_type { get; set; }
		public object[] api_df_list { get; set; }
		public object[] api_si_list { get; set; }
		public object[] api_cl_list { get; set; }
		public object[] api_damage { get; set; }
	}

	public class Api_Raigeki
	{
		public int[] api_frai { get; set; }
		public int[] api_erai { get; set; }
		public int[] api_fdam { get; set; }
		public float[] api_edam { get; set; }
		public int[] api_fydam { get; set; }
		public int[] api_eydam { get; set; }
		public int[] api_fcl { get; set; }
		public int[] api_ecl { get; set; }
	}

	public class Api_Hougeki2
	{
		public int[] api_at_list { get; set; }
		public int[] api_at_type { get; set; }
		public object[] api_df_list { get; set; }
		public object[] api_si_list { get; set; }
		public object[] api_cl_list { get; set; }
		public object[] api_damage { get; set; }
	}

	public class Api_Hougeki3
	{
		public int[] api_at_list { get; set; }
		public int[] api_at_type { get; set; }
		public object[] api_df_list { get; set; }
		public object[] api_si_list { get; set; }
		public object[] api_cl_list { get; set; }
		public object[] api_damage { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
