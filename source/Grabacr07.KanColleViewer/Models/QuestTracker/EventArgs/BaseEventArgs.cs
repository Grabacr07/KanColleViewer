using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleViewer.Models.QuestTracker.Model;
using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.EventArgs
{
	internal class BaseEventArgs
	{
		public bool IsSuccess { get; set; }

		public BaseEventArgs(bool IsSuccess)
		{
			this.IsSuccess = IsSuccess;
		}
	}
}
