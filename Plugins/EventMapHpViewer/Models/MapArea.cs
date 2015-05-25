using Grabacr07.KanColleWrapper.Models;
using EventMapHpViewer.Models.Raw;

namespace EventMapHpViewer.Models
{
	public class MapArea : RawDataWrapper<kcsapi_mst_maparea>, IIdentifiable
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public MapArea(kcsapi_mst_maparea maparea)
			: base(maparea)
		{
			this.Id = maparea.api_id;
			this.Name = maparea.api_name;
		}

		#region static members

		private static MapArea dummy = new MapArea(new kcsapi_mst_maparea()
		{
			api_id = 0,
			api_name = "？？？",
		});

		public static MapArea Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
