using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	public class kcsapi_data_airbaseattack : kcsapi_data_kouku
	{
		public int api_base_id { get; set; }
		public int[] api_stage_flag { get; set; }
		public kcsapi_data_squadron_plane[] api_squadron_plane { get; set; }
	}
	public class kcsapi_data_airbase_injection : kcsapi_data_kouku
	{
		public kcsapi_data_squadron_plane[] api_air_base_data { get; set; }
	}
	public class kcsapi_data_squadron_plane
	{
		public int api_mst_id { get; set; }
		public int api_count { get; set; }
	}
}
