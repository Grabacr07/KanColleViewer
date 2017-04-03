using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;
using Grabacr07.KanColleViewer.QuestTracker.Models.Model;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 정예 "97식함공" 부대의 편성 (텐잔 무라타대)
	/// </summary>
	internal class F20 : ITracker
	{
		private int lastCount = 0;
		private QuestProgressType lastProgress = QuestProgressType.None;
		private readonly int max_count = 3;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 623;
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
					83,  // 赤城
					277, // 赤城改
					110, // 翔鶴
					288, // 翔鶴改
					461, // 翔鶴改二
					466, // 翔鶴改二甲
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets[1];
				if (!flagshipTable.Any(x => x == (fleet?.Ships[0]?.Info.Id ?? 0))) return; // 쇼카쿠/아카기 비서함

				var slotitems = manager.slotitemTracker.SlotItems;
				count = count.Add(args.itemList.Count(x => (slotitems[x]?.Info.Id ?? 0) == 16)) // 97식 함상공격기
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
			return count >= max_count ? "완료" : "쇼카쿠/아카기 비서함, 97식 함상공격기 폐기 " + count.ToString() + " / " + max_count.ToString();
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

		public void CheckOverUnder(QuestProgressType progress)
		{
			if (lastCount == count && lastProgress == progress) return;
			lastProgress = progress;

			int cut50 = (int)Math.Ceiling(max_count * 0.5);
			int cut80 = (int)Math.Ceiling(max_count * 0.8);

			switch (progress)
			{
				case QuestProgressType.None:
					if (count >= cut50) count = cut50 - 1;
					break;
				case QuestProgressType.Progress50:
					if (count >= cut80) count = cut80 - 1;
					else if (count < cut50) count = cut50;
					break;
				case QuestProgressType.Progress80:
					if (count < cut80) count = cut80;
					break;
				case QuestProgressType.Complete:
					count = max_count;
					break;
			}
			lastCount = count;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}
	}
}
