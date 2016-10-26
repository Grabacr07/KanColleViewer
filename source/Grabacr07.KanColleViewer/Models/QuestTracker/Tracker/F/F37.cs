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
	/// 항공기지설영 사전준비
	/// </summary>
	internal class F37 : ITracker
	{
		private readonly int max_count = 6;
		private int count, count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 641;
		public QuestType Type => QuestType.OneTime;
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

				var homeportSlotitems = homeport.Itemyard.SlotItems;
				count = count.Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 75)).Max(2); // 드럼통
				count_1 = slotitems.Count(x => x.Info.Id == 37).Max(2); // 7.7mm 기관총
				count_2 = slotitems.Count(x => x.Info.Id == 19).Max(2); // 96식 함전

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
			manager.EquipEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var slotitems = homeport.Itemyard.SlotItems.Select(x => x.Value).ToArray();
				slotitems = slotitems.Where(x => homeport.Organization.Ships.Any(y => !y.Value.Slots.Select(z => z.Item.Id).Contains(x.Id)))
					.Where(x => x.RawData.api_locked == 0).ToArray(); // 장비중이지 않고 잠기지 않은 장비들

				count_1 = slotitems.Count(x => x.Info.Id == 37).Max(2); // 7.7mm 기관총
				count_2 = slotitems.Count(x => x.Info.Id == 19).Max(2); // 96식 함전

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
			return count * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count + count_1 + count_2) >= max_count ? "완료" :
				"7.7mm 기관총 소지 " + count_1.ToString() + "/2, "
				+ "96식 함상전투기 소지 " + count_2.ToString() + "/2, "
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
