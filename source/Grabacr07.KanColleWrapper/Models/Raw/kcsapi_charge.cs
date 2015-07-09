using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_charge
	{
		public kcsapi_charge_ship[] api_ship { get; set; }
		public int[] api_material { get; set; }
		public int api_use_bou { get; set; }
	}
	public class kcsapi_charge_ship
	{
		public int api_id { get; set; }
		public int api_fuel { get; set; }
		public int api_bull { get; set; }
		public int[] api_onslot { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
