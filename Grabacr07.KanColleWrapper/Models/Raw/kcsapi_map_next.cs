
namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_map_next : kcsapi_map_start
	{
		public int api_comment_kind { get; set; }
		public int api_production_kind { get; set; }

	}
	public class api_happening
	{
		public int api_type { get; set; }
		public int api_count { get; set; }//빠져나간 양
		public int api_usemst { get; set; }
		public int api_mst_id { get; set; }//종류. 2 탄
		public int api_icon_id { get; set; }
		public int api_dentan { get; set; }
	}
	public class api_enemy
	{
		public int api_enemy_id { get; set; }
		public int api_result { get; set; }
		public string api_result_str { get; set; }
	}
	public class api_itemget
	{
		public int api_usemst { get; set; }
		public int api_id { get; set; }//종류. 1 연
		public int api_getcount { get; set; }//얻은양
		public string api_name { get; set; }
		public int api_icon_id { get; set; }
	}
}
