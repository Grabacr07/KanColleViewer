using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Contents.Fleets
{
	public class PlaneViewModel : ViewModel
	{
		private SlotItemInfo slotItemInfo;

		public string Name
		{
			get { return this.slotItemInfo.Name; }
		}

		public int Count { get; private set; }

		public PlaneViewModel(SlotItemInfo info, int count)
		{
			this.slotItemInfo = info;
			this.Count = count;
		}
	}
}
