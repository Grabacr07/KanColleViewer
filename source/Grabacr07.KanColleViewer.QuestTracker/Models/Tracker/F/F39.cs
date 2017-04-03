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
	/// 주력 육공의 조달
	/// </summary>
	internal class F39 : NoOverUnderTracker, ITracker
	{
		private readonly int max_count = 5;
		private int count, count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 643;
		public QuestType Type => QuestType.Other; // 계절
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				count = count.Add(args.itemList.Count(x => x == 168)).Max(2); // 96식 육공
				count_1 = slotitems.Count(x => x.Info.Id == 37).Max(1); // 7.7mm 기관총
				count_2 = slotitems.Count(x => x.Info.Id == 16).Max(2); // 97식 함공

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
			manager.EquipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				count_1 = slotitems.Count(x => x.Info.Id == 37).Max(1); // 7.7mm 기관총
				count_2 = slotitems.Count(x => x.Info.Id == 16).Max(2); // 97식 함공

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count = count_1 = count_2 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (count + count_1 + count_2) * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count + count_1 + count_2) >= max_count ? "완료" :
				"96식 육상공격기 소지 " + count_1.ToString() + "/1, "
				+ "97식 함상공격기 소지 " + count_2.ToString() + "/2, "
				+ "영식 함상전투기 21형 폐기 " + count.ToString() + "/2";
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
