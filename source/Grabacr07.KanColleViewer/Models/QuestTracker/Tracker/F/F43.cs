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
	/// 중부해역 기지항공대 전개!
	/// </summary>
	internal class F43 : ITracker
	{
		private readonly int max_count = 4;
		private int count, count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 647;
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

				count_1 = homeport.Materials.Fuel >= 1200 ? 1 : 0; // 연료
				count_2 = homeport.Materials.Bauxite >= 3000 ? 1 : 0; // 보크사이트

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				var homeportSlotitems = homeport.Itemyard.SlotItems;
				count = args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 75).Max(2); // 드럼통

				count_1 = homeport.Materials.Fuel >= 1200 ? 1 : 0; // 연료
				count_2 = homeport.Materials.Bauxite >= 3000 ? 1 : 0; // 보크사이트

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
				"연료 1200 소지 " + (count_1 == 0 ? "X" : "OK") + ", "
				+ "보크사이트 3000 소지 " + (count_2 == 0 ? "X" : "OK") + ", "
				+ "드럼통 폐기 " + count.ToString() + "/2";
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
