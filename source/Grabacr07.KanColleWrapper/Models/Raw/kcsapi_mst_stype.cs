using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_mst_stype
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
		[DataMember(Name = "24")]
		public int _24 { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
