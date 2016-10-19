using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Tracker
{
	/// <summary>
	/// 특주가구의 조달
	/// </summary>
	internal class F42 : ITracker
	{
		private readonly int max_count = 6;
		private int count, count_1, count_2, count_3;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 646;
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

				count_1 = homeport.Admiral.RawData.api_fcoin >= 5000 ? 1 : 0; // 가구코인
				count_2 = slotitems.Count(x => x.Info.Id == 39).Max(2); // 25mm 연장기관총
				count_3 = slotitems.Count(x => x.Info.Id == 40).Max(2); // 25mm 삼연장기관총

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				count = args.itemList.Count(x => x == 49).Max(1); // 25mm 단장기관총

				count_1 = homeport.Admiral.RawData.api_fcoin >= 5000 ? 1 : 0; // 가구코인
				count_2 = slotitems.Count(x => x.Info.Id == 39).Max(2); // 25mm 연장기관총
				count_3 = slotitems.Count(x => x.Info.Id == 40).Max(2); // 25mm 삼연장기관총

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count = count_1 = count_2 = count_3 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (count + count_1 + count_2 + count_3) * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count_1 + count_2 + count_3) >= max_count ? "완료" :
				"가구코인 5000 소지 " + (count_1 == 0 ? "X" : "OK") + ", "
				+ "25mm 연장기관총 소지 " + count_2.ToString() + "/2, "
				+ "25mm 삼연장기관총 소지" + count_3.ToString() + "/2, "
				+ "25mm 단장기관총 폐기 " + count.ToString() + "/1";
		}

		public string SerializeData()
		{
			return count.ToString();
		}

		public void DeserializeData(string data)
		{
			count = 0;
			int.TryParse(data, out count);
		}
	}
}
