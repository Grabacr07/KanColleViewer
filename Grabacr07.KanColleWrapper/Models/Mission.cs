using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	[DebuggerDisplay("[{Id}] {Title} - {Detail}")]
	public class Mission : RawDataWrapper<kcsapi_mission>, IIdentifiable
	{
		public int Id { get; private set; }

		public string Title { get; private set; }

		public string Detail { get; private set; }

		public Mission(kcsapi_mission mission)
			: base(mission)
		{
			this.Id = mission.api_id;
			this.Title = mission.api_name;
			this.Detail = mission.api_details;
		}
	}
}
