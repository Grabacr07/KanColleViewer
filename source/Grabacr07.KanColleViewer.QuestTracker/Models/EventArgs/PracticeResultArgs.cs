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
	internal class PracticeResultEventArgs : BaseEventArgs
	{
		public PracticeResultEventArgs(kcsapi_practice_result data) : base("SAB".Contains(data.api_win_rank))
		{
		}
	}
}
