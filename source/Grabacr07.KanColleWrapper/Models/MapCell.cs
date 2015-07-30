using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class MapCell : RawDataWrapper<kcsapi_mst_mapcell>, IIdentifiable
	{
		public int ColorNo { get; }

		public int Id { get; }

		public int MapInfoId { get; }

		public MapInfo MapInfo { get; internal set; }

		public int MapAreaId { get; }

		public int MapInfoIdInEachMapArea { get; }

		public int IdInEachMapInfo { get; }

		public MapCell(kcsapi_mst_mapcell cell)
			: base(cell)
		{
			this.ColorNo = cell.api_color_no;
			this.Id = cell.api_id;
			this.MapInfoId = cell.api_map_no;
			this.MapAreaId = cell.api_maparea_id;
			this.MapInfoIdInEachMapArea = cell.api_mapinfo_no;
			this.IdInEachMapInfo = cell.api_no;
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Map = {this.MapAreaId}-{this.MapInfoIdInEachMapArea}, CellNo = {this.IdInEachMapInfo}, ColorNo = {this.ColorNo}";
		}

		#region static members

		public static MapCell Dummy { get; } = new MapCell(new kcsapi_mst_mapcell
		{
			api_id = 0,
			api_color_no = 0,
			api_maparea_id = 0,
			api_no = 0,
			api_mapinfo_no = 0,
			api_map_no = 0,
		})
		{
			MapInfo = MapInfo.Dummy,
		};

		#endregion
	}
}
