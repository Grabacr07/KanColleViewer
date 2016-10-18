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
	/// 기종 전환 (영전52갑형 이와이소대)
	/// </summary>
	internal class F29 : ITracker
	{
		private readonly int max_count = 2;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 632;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var flagshipTable = new int[]
				{
					111, // 瑞鶴
					112, // 瑞鶴改
					462, // 瑞鶴改二
					467, // 瑞鶴改二甲
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets[0];
				if (!flagshipTable.Any(x => x == fleet.Ships[0].Id)) return; // 즈이카쿠 비서함

				var slotitems = fleet.Ships[0].Slots;
				if (!slotitems.Any(x => x.Item.Info.Id == 155 && x.Item.Proficiency == 7)) return; // 숙련도max 영전 21형(이와이소대)

				count = count.Add(args.itemList.Count(x => x == 21)) // 영식 함전 52형
							.Max(max_count);

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return count * 100 / max_count;
		}

		public string GetProgressText()
		{
			return count >= max_count ? "완료" : "즈이카쿠 비서함, 숙련도max 영식 함상전투기 21형(이와이소대) 장착, 영식 함상전투기 52형 폐기 " + count.ToString() + " / " + max_count.ToString();
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
