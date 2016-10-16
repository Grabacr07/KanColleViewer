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
	/// 제1수뢰전대 북방돌입준비!
	/// </summary>
	internal class A54 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 5;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 156;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

				var flagshipTable = new int[]
				{
					114, // 阿武隈
					290, // 阿武隈改
					200, // 阿武隈改二
				};
				var shipTable = new int[]
				{
					114, // 阿武隈
					290, // 阿武隈改
					200, // 阿武隈改二
					35,  // 響
					41,  // 初霜
					40,  // 若葉
					46,  // 五月雨
					50,  // 島風
					235, // 響改
					241, // 初霜改
					240, // 若葉改
					246, // 五月雨改
					229, // 島風改
					147, // Верный
					419, // 初霜改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					if (!flagshipTable.Contains(ships[0].Info.Id)) continue; // 아부쿠마 기함

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

		public double GetProgress()
		{
			return (double)count / max_count * 100;
		}

		public string GetProgressText()
		{
			return count >= max_count ? "완료" : "아부쿠마 기함,히비키,하츠시모,와카바,사미다레,시마카제 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
