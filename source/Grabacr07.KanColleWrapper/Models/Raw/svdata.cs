using System;
using System.Collections.Generic;
using System.Linq;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class svdata
	{
		public int api_result { get; set; }
		public string api_result_msg { get; set; }
	}

	public class svdata<T> : svdata
	{
		public T api_data { get; set; }
		public kcsapi_deck[] api_data_deck { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
