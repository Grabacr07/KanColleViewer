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
	/// 정예 함전대의 신 편성 (영전21형 숙련, 월간)
	/// </summary>
	internal class F22 : ITracker
	{
		private readonly int max_count = 3;
		private int count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 626;
		public QuestType Type => QuestType.Monthly;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var flagshipTable = new int[]
				{
					89,  // 鳳翔
					285, // 鳳翔改
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets[0];
				if (!flagshipTable.Any(x => x == (fleet?.Ships[0]?.Info.Id ?? 0))) return; // 호쇼 비서함

				var slotitems = fleet?.Ships[0]?.Slots;
				if (!slotitems.Any(x => x.Item.Info.Id == 20 && x.Item.Proficiency == 7)) return; // 숙련도max 영식함전21형

				var homeportSlotitems = KanColleClient.Current.Homeport.Itemyard.SlotItems;
				count_1 = count_1.Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 20)).Max(2); // 영식함전21형
				count_2 = count_2.Add(args.itemList.Count(x => (homeportSlotitems[x]?.Info.Id ?? 0) == 19)).Max(1); // 96식함전

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
			return (count_1 + count_2) >= max_count ? "완료" : "호쇼 비서함, 숙련도max 영식 함상전투기 21형 장착, 영식 함상전투기 21형 폐기 " + count_1.ToString() + "/2, 96식 함상전투기 폐기 " + count_2.ToString() + "/1";
		}

		public string SerializeData()
		{
			return count_1.ToString()+","+count_2.ToString();
		}

		public void DeserializeData(string data)
		{
			count_1 = count_2 = 0;

			var part = data.Split(',');
			if (part.Length != 2) return;

			int.TryParse(part[0], out count_1);
			int.TryParse(part[1], out count_2);
		}
	}
}
