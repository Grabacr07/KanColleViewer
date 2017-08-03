using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 잠수함 파견 작전에 의한 제트기 기술 입수
	/// </summary>
	internal class D21 : NoOverUnderTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count_1, count_2, count_3, count_4;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 423;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.MissionResultEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				if (!args.IsSuccess) return;

				var homeport = KanColleClient.Current.Homeport;

				count_1 = homeport.Materials.Steel >= 5000 ? 1 : 0; // 강재
				count_2 = homeport.Materials.Bauxite >= 1500 ? 1 : 0; // 보크사이트

				if (args.Name == "潜水艦派遣作戦") count_3 = count_3.Add(1).Max(1);
				if (args.Name == "海外艦との接触") count_4 = count_4.Add(1).Max(1);

				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
		}

		public void ResetQuest()
		{
			count_1 = count_2 = count_3 = count_4 = 0;
			ProcessChanged?.Invoke(this, emptyEventArgs);
		}

		public int GetProgress()
		{
			return (count_1 + count_2 + count_3 + count_4) * 100 / max_count;
		}

		public string GetProgressText()
		{
			return (count_1 + count_2) >= max_count ? "완료" :
				"강재 5000 소지 " + (count_1 == 0 ? "X" : "OK") + ", "
				+ "보크사이트 1500 소지 " + (count_2 == 0 ? "X" : "OK") + ", "
				+ "잠수함파견작전(30) 원정 성공 " + count_3.ToString() + "/1, "
				+ " 해외함과의 접촉(31) 원정 성공 " + count_4.ToString() + "/1";
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
