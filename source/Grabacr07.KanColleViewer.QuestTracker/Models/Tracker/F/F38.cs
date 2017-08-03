using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 육공대의 증세
	/// </summary>
	internal class F38 : NoSerializeOverUnderTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 642;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.EquipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				count_1 = slotitems.Count(x => x.Info.Id == 37).Max(2); // 7.7mm 기관총
				count_2 = slotitems.Count(x => x.Info.Id == 23).Max(2); // 99식 함폭

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count_1 = count_2 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (count_1 + count_2) * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count_1 + count_2) >= max_count ? "완료" :
				"7.7mm 기관총 소지 " + count_1.ToString() + "/2, "
				+ "99식 함상폭격기 소지 " + count_2.ToString() + "/2";
		}
	}
}
