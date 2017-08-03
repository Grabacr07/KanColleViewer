using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Tracker
{
	/// <summary>
	/// 경쾌한 수상반격부대를 편성하라!
	/// </summary>
	internal class A46 : NoSerializeOverUnderTracker, ITracker
	{
		private readonly int max_count = 6;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 148;
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
					49,  // 霞
					253, // 霞改
					464, // 霞改二
					470, // 霞改二乙
				};
				var shipTable = new int[]
				{
					49,  // 霞
					64,  // 足柄
					253, // 霞改
					267, // 足柄改
					464, // 霞改二
					193, // 足柄改二
					470, // 霞改二乙
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length != 0) continue;

					if (!flagshipTable.Contains((ships[0]?.Info.Id ?? 0))) continue; // 기함 카스미

					count = Math.Max(
						count,
						(
							ships.Count(x => shipTable.Contains(x.Info.Id))
								+ ships.Count(x => x.Info.ShipType.Id == 3).Max(1)
								+ ships.Count(x => x.Info.ShipType.Id == 2 && !shipTable.Contains(x.Info.Id)).Max(3)
								// 카스미 제외 구축
						).Max(5)
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
			return count >= max_count ? "완료" : "카스미 기함,아시가라,경순 1척,구축 3척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
