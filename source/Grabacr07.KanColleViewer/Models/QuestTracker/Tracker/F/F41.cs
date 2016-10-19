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
	/// 해상보급 물자의 조달
	/// </summary>
	internal class F41 : ITracker
	{
		private readonly int max_count = 6;
		private int count, count_1, count_2, count_3, count_4;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 645;
		public QuestType Type => QuestType.Monthly;
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

				count_1 = homeport.Materials.Fuel >= 750 ? 1 : 0; // 연료
				count_2 = homeport.Materials.Ammunition >= 750 ? 1 : 0; // 탄약
				count_3 = slotitems.Count(x => x.Info.Id == 75).Max(2); // 드럼통
				count_4 = slotitems.Count(x => x.Info.Id == 36).Max(1); // 91식 철갑탄

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				count = args.itemList.Count(x => x == 35).Max(1); // 삼식탄

				count_1 = homeport.Materials.Fuel >= 750 ? 1 : 0; // 연료
				count_2 = homeport.Materials.Ammunition >= 750 ? 1 : 0; // 탄약
				count_3 = slotitems.Count(x => x.Info.Id == 75).Max(2); // 드럼통
				count_4 = slotitems.Count(x => x.Info.Id == 36).Max(1); // 91식 철갑탄

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count = count_1 = count_2 = count_3 = count_4 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (count + count_1 + count_2 + count_3 + count_4) * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count_1 + count_2 + count_3 + count_4) >= max_count ? "완료" :
				"연료 750 소지 " + (count_1 == 0 ? "X" : "OK") + ", "
				+ "탄약 750 소지 " + (count_2 == 0 ? "X" : "OK") + ", "
				+ "드럼통 소지 " + count_3.ToString() + "/2, "
				+ "91식 철갑탄 소지" + count_4.ToString() + "/1, "
				+ "삼식탄 폐기 " + count.ToString() + "/1";
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
