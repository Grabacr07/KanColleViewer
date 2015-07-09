using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_powerup
	{
		public int api_powerup_flag { get; set; }

		public kcsapi_ship2 api_ship { get; set; }

		public kcsapi_deck[] api_deck { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
