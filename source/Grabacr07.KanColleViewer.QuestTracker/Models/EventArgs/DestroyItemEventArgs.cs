using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Specialized;
using Grabacr07.KanColleViewer.QuestTracker.Models.Model;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.EventArgs
{
	internal class DestroyItemEventArgs
	{
		public int[] itemList { get; set; }

		public DestroyItemEventArgs(NameValueCollection request, kcsapi_destroyitem2 data)
		{
			itemList = request["api_slotitem_ids"].Split(',')
				.Select(int.Parse).ToArray();
		}
	}
}
