using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class MapCellEvent : RawDataWrapper<kcsapi_map_next>
	{
		public bool IsBattle
		{
			get { return this.RawData.api_enemy != null; }
		}

		public bool IsItemGet
		{
			get { return this.RawData.api_itemget != null; }
		}

		public MapCellEvent(kcsapi_map_next rawData) : base(rawData) { }
	}
}
