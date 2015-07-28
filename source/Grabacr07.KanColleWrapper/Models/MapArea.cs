using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class MapArea : RawDataWrapper<kcsapi_mst_maparea>, IIdentifiable
	{
		public int Id { get; }

		public string Name { get; }

		public MasterTable<MapInfo> MapInfos { get; }

		public MapArea(kcsapi_mst_maparea maparea, MasterTable<MapInfo> mapInfos)
			: base(maparea)
		{
			this.Id = maparea.api_id;
			this.Name = maparea.api_name;
			this.MapInfos = new MasterTable<MapInfo>(mapInfos.Values.Where(x => x.MapAreaId == maparea.api_id));
			foreach (var cell in this.MapInfos.Values)
				cell.MapArea = this;
		}
		
		public override string ToString()
		{
			return $"ID = {this.Id}, Name = {this.Name}";
		}

		#region static members

		public static MapArea Dummy { get; } = new MapArea(new kcsapi_mst_maparea
		{
		    api_id = 0,
		    api_name = "？？？",
		}, new MasterTable<MapInfo>());

	    #endregion
	}
}
