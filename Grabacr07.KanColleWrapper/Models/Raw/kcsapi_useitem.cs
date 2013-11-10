using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models.Raw
{
	// ReSharper disable InconsistentNaming
	public class kcsapi_useitem
	{
		public int api_member_id { get; set; }
		public int api_id { get; set; }
		public int api_value { get; set; }
		public int api_usetype { get; set; }
		public int api_category { get; set; }
		public string api_name { get; set; }
		public string[] api_description { get; set; }
		public int api_price { get; set; }
		public int api_count { get; set; }
	}
	// ReSharper restore InconsistentNaming
}
