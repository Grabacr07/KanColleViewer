
namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_mapinfo
	{
		public int api_id { get; set; }
		public int api_cleared { get; set; }
		public int api_exboss_flag { get; set; }
		public api_eventmap api_eventmap { get; set; }
	}
	public class api_eventmap
	{
		public int api_now_maphp { get; set; }
		public int api_max_maphp { get; set; }
		public int api_state { get; set; }
	}
	/// <summary>
	/// NOT API. for calculate
	/// </summary>
	public class Maplists
	{
		public int MaxHp { get; set; }
		public int NowHp { get; set; }
		public int ApiId { get; set; }
		public int Num { get; set; }
	}
}
