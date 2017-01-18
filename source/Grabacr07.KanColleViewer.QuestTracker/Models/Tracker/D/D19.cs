using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 기동부대의 운용을 강화하라
	/// </summary>
	internal class D19 : ITracker
	{
		private readonly int max_count = 2;
		private int count_1, count_2;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 420;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.MissionResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;

				if (args.Name == "MO作戦") count_1 = count_1.Add(1).Max(1);
				if (args.Name == "敵母港空襲作戦") count_2 = count_2.Add(1).Max(1);

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
			return (count_1 + count_2) >= max_count ? "완료" : "MO작전(35) 원정 성공 " + count_1.ToString() + "/1, "
				+ " 적 모항 공습작전(26) 원정 성공 " + count_2.ToString()+"/1";
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
