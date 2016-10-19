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
	/// 제16전대(제1차)를 편성하라!
	/// </summary>
	internal class A58 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 3;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 163;
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
					64,  // 足柄
					267, // 足柄改
					193, // 足柄改二
				};
				var shipTable = new int[]
				{
					64,  // 足柄
					267, // 足柄改
					193, // 足柄改二
					99,  // 球磨
					21,  // 長良
					215, // 球磨改
					218, // 長良改
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					if (!flagshipTable.Contains(ships[0].Info.Id)) continue; // 아시가라 기함

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
			return count >= max_count ? "완료" : "아시가라 기함,쿠마,나가라 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
