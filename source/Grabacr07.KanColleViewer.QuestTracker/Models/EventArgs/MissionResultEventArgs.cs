using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleViewer.QuestTracker.Models.Model;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.EventArgs
{
	internal class MissionResultEventArgs : BaseEventArgs
	{
		public string Name { get; set; }

		public MissionResultEventArgs(kcsapi_mission_result data) : base(data.api_clear_result > 0)
		{
			Name = data.api_quest_name;
		}
	}
}
