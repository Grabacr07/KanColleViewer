
namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_ship3
	{
		public kcsapi_ship2[] api_ship_data { get; set; }
		public kcsapi_deck[] api_deck_data { get; set; }

		// 今のところ必要ないので
		//public kcsapi_slot_data api_slot_data { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
