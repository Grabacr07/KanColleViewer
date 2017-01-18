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
	/// 제5함대를 편성하라!
	/// </summary>
	internal class A29 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 129;
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
					63,  // 那智
					64,  // 足柄
					100, // 多摩
					101, // 木曾
					266, // 那智改
					267, // 足柄改
					216, // 多摩改
					217, // 木曾改
					192, // 那智改二
					193, // 足柄改二
					146, // 木曾改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Info.Id))
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
			return count >= max_count ? "완료" : "나치,아시가라,타마,키소 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
