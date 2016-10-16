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
	/// 콩고급으로 편성된 고속전함부대를 편성하라!
	/// </summary>
	internal class A16 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 118;
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
					78,  // 金剛
					86,  // 比叡
					79,  // 榛名
					85,  // 霧島
					149, // 金剛改
					150, // 比叡改
					151, // 榛名改
					152, // 霧島改
					209, // 金剛改二
					210, // 比叡改二
					211, // 榛名改二
					212, // 霧島改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Id)).Max(4)
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
			return count >= max_count ? "완료" : "콩고,히에이,하루나,키리시마 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
