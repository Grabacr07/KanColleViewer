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
	/// 상륙전용신장비의 조달
	/// </summary>
	internal class F33 : ITracker
	{
		private readonly int max_count = 4;
		private int count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 636;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.DestoryItemEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				count_1 = count_1.Add(args.itemList.Count(x => x == 37)).Max(2);
				count_2 = count_2.Add(args.itemList.Count(x => x == 38)).Max(2);

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
			return (count_1 + count_2) >= max_count ? "완료" : "7.7mm 기관총 폐기 " + count_1.ToString() + "/2, 12.7mm 단장기관총 폐기 " + count_2.ToString()+"/2";
		}

		public string SerializeData()
		{
			return count_1.ToString() + "," + count_2.ToString();
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
