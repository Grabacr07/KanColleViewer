
namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_battle
	{
		public int api_dock_id { get; set; }
		public api_opening_atack api_opening_atack;
		public api_hougeki1 api_hougeki1;
		public api_hougeki2 api_hougeki2;
		public api_hougeki3 api_hougeki3;
		public api_kouku api_kouku;
		public api_kouku2 api_kouku2;
		public api_raigeki api_raigeki;
		public int[] api_maxhps;//it works well
		public int[] api_nowhps;
		//combined전용
		public int[] api_nowhps_combined;
		public int[] api_maxhps_combined;
		//적 ID
		public int[] api_ship_ke;
		//적 Lv
		public int[] api_ship_lv;
		//지원함대
		public int api_support_flag;
		public api_support_info api_support_info;
	}
	public class api_support_info
	{
		public api_support_hourai api_support_hourai;
		public api_support_airatack api_support_airatack;
	}
	public class api_support_hourai
	{
		public int api_deck_id;
		public int[] api_ship_id;
		public int[] api_undressing_flag;
		public int[] api_cl_list;
		public decimal[] api_damage;

	}
	public class api_support_airatack
	{
		public int api_deck_id;
		public int[] api_ship_id;
		public int[] api_undressing_flag;
		public int[] api_stage_flag;
		public object[] api_plane_from;
		public api_stage3 api_stage3;
	}
	public class kcsapi_midnight_battle
	{
		public int api_deck_id { get; set; }
		public api_hougeki api_hougeki;
		public api_stage3 api_stage3;
		public int[] api_maxhps;//it works well
		public int[] api_nowhps;
		public int[] api_nowhps_combined;
		public int[] api_maxhps_combined;
		//적 ID
		public int[] api_ship_ke;
		//적 Lv
		public int[] api_ship_lv;

		//public int[] api_touch_plane;
		//public int[] api_flare_pos;
	}
	public class api_hougeki
	{
		public object[] api_damage;
		public object[] api_df_list;
	}

	public class api_hougeki1
	{
		public object[] api_damage;
		public object[] api_df_list;
	}
	public class api_hougeki2
	{
		public object[] api_damage;
		public object[] api_df_list;
	}
	public class api_hougeki3
	{
		public object[] api_damage;
		public object[] api_df_list;
	}
	public class api_raigeki
	{
		public decimal[] api_eydam;
		public int[] api_erai;
		//적 데미지
		public decimal[] api_fydam;
		public int[] api_frai;
	}
	public class api_kouku
	{
		public api_stage3 api_stage3;
		public api_Stage3_combined api_stage3_combined;
	}
	public class api_kouku2
	{
		public api_stage3 api_stage3;
		public api_Stage3_combined api_stage3_combined;
	}
	public class api_stage3
	{
		public int[] api_frai_flag;
		public int[] api_fbak_flag;
		public int[] api_fcl_flag;//사용 용도 불명
		public decimal[] api_fdam;

		//적군
		public int[] api_erai_flag;
		public int[] api_ebak_flag;
		public int[] api_ecl_flag;//사용 용도 불명
		public decimal[] api_edam;
	}
	public class api_Stage3_combined
	{
		public int[] api_frai_flag;
		public int[] api_fbak_flag;
		public int[] api_fcl_flag;
		public decimal[] api_fdam;

		//적군
		public int[] api_erai_flag;
		public int[] api_ebak_flag;
		public int[] api_ecl_flag;//사용 용도 불명
		public decimal[] api_edam;
	}
	/// <summary>
	/// 데미지와 타겟을 저장
	/// </summary>
	public class listup
	{
		public decimal Damage;
		public int Num;
	}
	public class api_opening_atack
	{
		public decimal[] api_eydam;
		public int[] api_erai;

		//개막전. 적 부분
		public decimal[] api_fydam;
		public int[] api_frai;
	}
}
