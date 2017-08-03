using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_mapinfo_airbase
	{
		public kcsapi_airbase_corps[] api_air_base { get; set; }
	}

	public class kcsapi_airbase_corps
	{
		public int api_area_id { get; set; }
		public int api_rid { get; set; }
		public string api_name { get; set; }
		public int api_distance { get; set; }
		public int api_action_kind { get; set; }
		public kcsapi_plane_info[] api_plane_info { get; set; }
	}

	public class kcsapi_airbase_corps_supply
	{
		public int api_after_fuel { get; set; }
		public int api_after_bauxite { get; set; }
		public int api_distance { get; set; }
		public kcsapi_plane_info[] api_plane_info { get; set; }
	}

	public class kcsapi_airbase_corps_set_plane
	{
		public int api_after_bauxite { get; set; }
		public int api_distance { get; set; }
		public kcsapi_plane_info[] api_plane_info { get; set; }
	}

	public class kcsapi_plane_info
	{
		public int api_squadron_id { get; set; }
		public int api_state { get; set; } // ?
		public int api_slotid { get; set; }
		public int api_count { get; set; }
		public int api_max_count { get; set; }
		public int api_cond { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
