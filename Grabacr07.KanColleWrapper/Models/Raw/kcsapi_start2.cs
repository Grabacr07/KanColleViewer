using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_start2
	{
		public kcsapi_mst_ship[] api_mst_ship { get; set; }
		public kcsapi_mst_slotitem[] api_mst_slotitem { get; set; }
		public kcsapi_mst_useitem[] api_mst_useitem { get; set; }
		public kcsapi_mst_stype[] api_mst_stype { get; set; }
		public kcsapi_mst_slotitem_equiptype[] api_mst_slotitem_equiptype { get; set; }

		// ↓ とりあえずそのまま。

		public Api_Mst_Shipgraph[] api_mst_shipgraph { get; set; }
		public Api_Mst_Slotitemgraph[] api_mst_slotitemgraph { get; set; }
		public Api_Mst_Furniture[] api_mst_furniture { get; set; }
		public Api_Mst_Furnituregraph[] api_mst_furnituregraph { get; set; }
		public Api_Mst_Payitem[] api_mst_payitem { get; set; }
		public Api_Mst_Item_Shop api_mst_item_shop { get; set; }
		public Api_Mst_Maparea[] api_mst_maparea { get; set; }
		public Api_Mst_Mapinfo[] api_mst_mapinfo { get; set; }
		public Api_Mst_Mapbgm[] api_mst_mapbgm { get; set; }
		public Api_Mst_Mapcell[] api_mst_mapcell { get; set; }
		public Api_Mst_Mission[] api_mst_mission { get; set; }
		public Api_Mst_Const api_mst_const { get; set; }
		public Api_Mst_Shipupgrade[] api_mst_shipupgrade { get; set; }
	}


	public class Api_Mst_Item_Shop
	{
		public int[] api_cabinet_1 { get; set; }
		public int[] api_cabinet_2 { get; set; }
	}

	public class Api_Mst_Const
	{
		public Api_Boko_Max_Ships api_boko_max_ships { get; set; }
	}

	public class Api_Boko_Max_Ships
	{
		public string api_string_value { get; set; }
		public int api_int_value { get; set; }
	}

	public class Api_Mst_Ship
	{
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public string api_name { get; set; }
		public string api_yomi { get; set; }
		public int api_stype { get; set; }
		public int api_ctype { get; set; }
		public int api_cnum { get; set; }
		public string api_enqflg { get; set; }
		public int api_afterlv { get; set; }
		public string api_aftershipid { get; set; }
		public int[] api_taik { get; set; }
		public int[] api_souk { get; set; }
		public int[] api_tous { get; set; }
		public int[] api_houg { get; set; }
		public int[] api_raig { get; set; }
		public int[] api_baku { get; set; }
		public int[] api_tyku { get; set; }
		public int[] api_atap { get; set; }
		public int[] api_tais { get; set; }
		public int[] api_houm { get; set; }
		public int[] api_raim { get; set; }
		public int[] api_kaih { get; set; }
		public int[] api_houk { get; set; }
		public int[] api_raik { get; set; }
		public int[] api_bakk { get; set; }
		public int[] api_saku { get; set; }
		public int[] api_sakb { get; set; }
		public int[] api_luck { get; set; }
		public int api_sokuh { get; set; }
		public int api_soku { get; set; }
		public int api_leng { get; set; }
		public int[] api_grow { get; set; }
		public int api_slot_num { get; set; }
		public int[] api_maxeq { get; set; }
		public int[] api_defeq { get; set; }
		public int api_buildtime { get; set; }
		public int[] api_broken { get; set; }
		public int[] api_powup { get; set; }
		public int[] api_gumax { get; set; }
		public int api_backs { get; set; }
		public string api_getmes { get; set; }
		public object api_homemes { get; set; }
		public object api_gomes { get; set; }
		public object api_gomes2 { get; set; }
		public string api_sinfo { get; set; }
		public int api_afterfuel { get; set; }
		public int api_afterbull { get; set; }
		public object[] api_touchs { get; set; }
		public object api_missions { get; set; }
		public object api_systems { get; set; }
		public int api_fuel_max { get; set; }
		public int api_bull_max { get; set; }
		public int api_voicef { get; set; }
	}

	public class Api_Mst_Shipgraph
	{
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public string api_filename { get; set; }
		public string api_version { get; set; }
		public int[] api_boko_n { get; set; }
		public int[] api_boko_d { get; set; }
		public int[] api_kaisyu_n { get; set; }
		public int[] api_kaisyu_d { get; set; }
		public int[] api_kaizo_n { get; set; }
		public int[] api_kaizo_d { get; set; }
		public int[] api_map_n { get; set; }
		public int[] api_map_d { get; set; }
		public int[] api_ensyuf_n { get; set; }
		public int[] api_ensyuf_d { get; set; }
		public int[] api_ensyue_n { get; set; }
		public int[] api_battle_n { get; set; }
		public int[] api_battle_d { get; set; }
		public int[] api_weda { get; set; }
		public int[] api_wedb { get; set; }
	}

	public class Api_Mst_Stype
	{
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public string api_name { get; set; }
		public int api_scnt { get; set; }
		public int api_kcnt { get; set; }
		public Api_Equip_Type api_equip_type { get; set; }
	}

	public class Api_Equip_Type
	{
		public int _1 { get; set; }
		public int _2 { get; set; }
		public int _3 { get; set; }
		public int _4 { get; set; }
		public int _5 { get; set; }
		public int _6 { get; set; }
		public int _7 { get; set; }
		public int _8 { get; set; }
		public int _9 { get; set; }
		public int _10 { get; set; }
		public int _11 { get; set; }
		public int _12 { get; set; }
		public int _13 { get; set; }
		public int _14 { get; set; }
		public int _15 { get; set; }
		public int _16 { get; set; }
		public int _17 { get; set; }
		public int _18 { get; set; }
		public int _19 { get; set; }
		public int _20 { get; set; }
		public int _21 { get; set; }
		public int _22 { get; set; }
		public int _23 { get; set; }
		public int _24 { get; set; }
		public int _25 { get; set; }
		public int _26 { get; set; }
		public int _27 { get; set; }
		public int _28 { get; set; }
		public int _29 { get; set; }
		public int _30 { get; set; }
		public int _31 { get; set; }
	}

	public class Api_Mst_Slotitem
	{
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public string api_name { get; set; }
		public int[] api_type { get; set; }
		public int api_taik { get; set; }
		public int api_souk { get; set; }
		public int api_houg { get; set; }
		public int api_raig { get; set; }
		public int api_soku { get; set; }
		public int api_baku { get; set; }
		public int api_tyku { get; set; }
		public int api_tais { get; set; }
		public int api_atap { get; set; }
		public int api_houm { get; set; }
		public int api_raim { get; set; }
		public int api_houk { get; set; }
		public int api_raik { get; set; }
		public int api_bakk { get; set; }
		public int api_saku { get; set; }
		public int api_sakb { get; set; }
		public int api_luck { get; set; }
		public int api_leng { get; set; }
		public int api_rare { get; set; }
		public int[] api_broken { get; set; }
		public string api_info { get; set; }
		public string api_usebull { get; set; }
	}

	public class Api_Mst_Slotitemgraph
	{
		public int api_id { get; set; }
		public int api_sortno { get; set; }
		public string api_filename { get; set; }
		public string api_version { get; set; }
	}

	public class Api_Mst_Furniture
	{
		public int api_id { get; set; }
		public int api_type { get; set; }
		public int api_no { get; set; }
		public string api_title { get; set; }
		public string api_description { get; set; }
		public int api_rarity { get; set; }
		public int api_price { get; set; }
		public int api_saleflg { get; set; }
	}

	public class Api_Mst_Furnituregraph
	{
		public int api_id { get; set; }
		public int api_type { get; set; }
		public int api_no { get; set; }
		public string api_filename { get; set; }
		public string api_version { get; set; }
	}

	public class Api_Mst_Useitem
	{
		public int api_id { get; set; }
		public int api_usetype { get; set; }
		public int api_category { get; set; }
		public string api_name { get; set; }
		public string[] api_description { get; set; }
		public int api_price { get; set; }
	}

	public class Api_Mst_Payitem
	{
		public int api_id { get; set; }
		public int api_type { get; set; }
		public string api_name { get; set; }
		public string api_description { get; set; }
		public int[] api_item { get; set; }
		public int api_price { get; set; }
	}

	public class Api_Mst_Maparea
	{
		public int api_id { get; set; }
		public string api_name { get; set; }
		public int api_type { get; set; }
	}

	public class Api_Mst_Mapinfo
	{
		public int api_id { get; set; }
		public int api_maparea_id { get; set; }
		public int api_no { get; set; }
		public string api_name { get; set; }
		public int api_level { get; set; }
		public string api_opetext { get; set; }
		public string api_infotext { get; set; }
		public int[] api_item { get; set; }
		public int? api_max_maphp { get; set; }
		public int? api_required_defeat_count { get; set; }
	}

	public class Api_Mst_Mapbgm
	{
		public int api_id { get; set; }
		public int api_maparea_id { get; set; }
		public int api_no { get; set; }
		public int[] api_map_bgm { get; set; }
		public int[] api_boss_bgm { get; set; }
	}

	public class Api_Mst_Mapcell
	{
		public int api_map_no { get; set; }
		public int api_maparea_id { get; set; }
		public int api_mapinfo_no { get; set; }
		public int api_id { get; set; }
		public int api_no { get; set; }
		public int api_color_no { get; set; }
	}

	public class Api_Mst_Mission
	{
		public int api_id { get; set; }
		public int api_maparea_id { get; set; }
		public string api_name { get; set; }
		public string api_details { get; set; }
		public int api_time { get; set; }
		public int api_difficulty { get; set; }
		public float api_use_fuel { get; set; }
		public float api_use_bull { get; set; }
		public int[] api_win_item1 { get; set; }
		public int[] api_win_item2 { get; set; }
	}

	public class Api_Mst_Shipupgrade
	{
		public int api_id { get; set; }
		public int api_original_ship_id { get; set; }
		public int api_upgrade_type { get; set; }
		public int api_upgrade_level { get; set; }
		public int api_drawing_count { get; set; }
		public int api_sortno { get; set; }
	}

	// ReSharper restore InconsistentNaming
}
