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
	/// 제1수뢰전대 북방재돌입준비!
	/// </summary>
	internal class A55 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 5;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 157;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

				var shipTable = new int[]
				{
					200, // 阿武隈改二
					35,  // 響
					133, // 夕雲
					135, // 長波
					132, // 秋雲
					50,  // 島風
					235, // 響改
					302, // 夕雲改
					304, // 長波改
					301, // 秋雲改
					229, // 島風改
					147, // Верный
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					if ((ships[0]?.Info.Id ?? 0) != 200) continue; // 아부쿠마改2 기함

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Id))
					);
				}

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
			return count >= max_count ? "완료" : "아부쿠마改2 기함,히비키,유구모,나가나미,아키구모,시마카제 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
