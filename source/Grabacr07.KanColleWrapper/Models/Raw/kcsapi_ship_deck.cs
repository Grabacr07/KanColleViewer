using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming

	public class kcsapi_ship_deck
	{
		public kcsapi_ship2[] api_ship_data { get; set; }
		public kcsapi_deck[] api_deck_data { get; set; }
	}
}