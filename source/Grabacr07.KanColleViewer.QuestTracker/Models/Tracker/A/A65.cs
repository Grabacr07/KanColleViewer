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
	/// 경쾌한 수상반격부대를 재편성하라!
	/// </summary>
	internal class A65 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 5;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 170;
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
					183, // 大淀
					425, // 朝霜
					253, // 霞改
					410, // 清霜
					267, // 足柄改
					321, // 大淀改
					344, // 朝霜改
					325, // 清霜改
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
			return count >= max_count ? "완료" : "카스미 기함,아시가라,오요도,아사시모,키요시모 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
