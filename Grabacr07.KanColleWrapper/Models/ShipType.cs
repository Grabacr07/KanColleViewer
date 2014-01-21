using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦種を表します。
	/// </summary>
	public class ShipType : RawDataWrapper<kcsapi_stype>, IIdentifiable
	{
		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
			get { return this.RawData.api_name; }
		}

		public int SortNumber
		{
			get { return this.RawData.api_sortno; }
		}

		public ShipType(kcsapi_stype rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\"", this.Id, this.Name);
		}

		#region static members

		private static readonly ShipType dummy = new ShipType(new kcsapi_stype
		{
			api_id = 999,
			api_sortno = 999,
			api_name = "不審船",
		});

		public static ShipType Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
