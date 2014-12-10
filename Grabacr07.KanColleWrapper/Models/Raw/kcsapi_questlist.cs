
namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_questlist
	{
		public int api_count { get; set; }
		public int api_page_count { get; set; }
		public int api_disp_page { get; set; }
		public kcsapi_quest[] api_list { get; set; }
		public int api_exec_count { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
