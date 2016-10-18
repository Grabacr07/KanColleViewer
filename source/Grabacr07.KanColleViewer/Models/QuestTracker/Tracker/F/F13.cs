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
	/// 기종 전환 (텐잔 토모나가대)
	/// </summary>
	internal class F13 : ITracker
	{
		private readonly int max_count = 2;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 614;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets[0];
				var slotitems = fleet.Ships[0].Slots;

				if (!slotitems.Any(x => x.Item.Info.Id == 93)) return; // 97식 함상공격기 (토모나가대)

				count = count.Add(args.itemList.Count(x => x == 17)) // 텐잔
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
			return count >= max_count ? "완료" : "비서함에 97식 함상공격기(토모나가대) 장착 후 텐잔 폐기 " + count.ToString() + " / " + max_count.ToString();
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
