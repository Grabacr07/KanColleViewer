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
		public int Id { get; }

		public string Title { get; }

		public string Detail { get; }

		public Mission(kcsapi_mission mission)
			: base(mission)
		{
			this.Id = mission.api_id;
			this.Title = KanColleClient.Current.Translations.GetTranslation(mission.api_name, TranslationType.ExpeditionTitle, false, this.RawData, mission.api_id);
			this.Detail = KanColleClient.Current.Translations.GetTranslation(mission.api_details, TranslationType.ExpeditionDetail, false, this.RawData, mission.api_id);
		}
	}
}
