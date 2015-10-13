using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class MapInfo : RawDataWrapper<kcsapi_mst_mapinfo>, IIdentifiable
	{
		public int Id { get; }

		public string Name { get; }

		public int MapAreaId { get;}

		public MapArea MapArea { get; internal set; }

		public int IdInEachMapArea { get;}

		public int Level { get; }

		public string OperationName { get; }

		public string OperationSummary { get; }

		public int RequiredDefeatCount { get; }

		public MasterTable<MapCell> MapCells { get; }

		public MapInfo(kcsapi_mst_mapinfo mapinfo, MasterTable<MapCell> mapCells)
			: base(mapinfo)
		{
			this.Id = mapinfo.api_id;
			this.Name = mapinfo.api_name;
			this.MapAreaId = mapinfo.api_maparea_id;
			this.IdInEachMapArea = mapinfo.api_no;
			this.Level = mapinfo.api_level;
			this.OperationName = mapinfo.api_opetext;
			this.OperationSummary = mapinfo.api_infotext;
			this.RequiredDefeatCount = mapinfo.api_required_defeat_count ?? 1;
			this.MapCells = new MasterTable<MapCell>(mapCells.Values.Where(x => x.MapInfoId == mapinfo.api_id));
			foreach (var cell in this.MapCells.Values)
				cell.MapInfo = this;
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = {this.Name}";
		}

		#region static members

		public static MapInfo Dummy { get; } = new MapInfo(new kcsapi_mst_mapinfo
		{
		    api_id = 0,
		    api_name = "？？？",
		    api_maparea_id = 0,
		    api_no = 0,
		    api_level = 0,
		}, new MasterTable<MapCell>())
	    {
		    MapArea = MapArea.Dummy,
	    };

	    #endregion
	}
}
