using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming

	public class kcsapi_start2
	{
		public kcsapi_mst_ship[] api_mst_ship { get; set; }
		public kcsapi_mst_slotitem_equiptype[] api_mst_slotitem_equiptype { get; set; }
		public kcsapi_mst_stype[] api_mst_stype { get; set; }
		public kcsapi_mst_slotitem[] api_mst_slotitem { get; set; }
		public kcsapi_mst_useitem[] api_mst_useitem { get; set; }
		public kcsapi_mst_maparea[] api_mst_maparea { get; set; }
		public kcsapi_mst_mapinfo[] api_mst_mapinfo { get; set; }
		public kcsapi_mission[] api_mst_mission { get; set; }
	}

	// ReSharper restore InconsistentNaming
}
