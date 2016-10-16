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
	/// 강행 고속 수송부대를 편성하라!
	/// </summary>
	internal class A68 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 5;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 173;
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
					158, // 川内改二
					469, // 江風改二
					145, // 時雨改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;
					if (ships[0].Info.Id != 158) continue;

					var y = ships.Count(x => shipTable.Contains(x.Info.Id)).Max(3);
					count = Math.Max(
						count,
						(y + (y < 3 ? 0 : ships.Count(x => x.Info.ShipType.Id == 2) - 3)).Max(5)
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
			return count >= max_count ? "완료" : "센다이改2 기함,카와카제改2,시구레改2,구축함 2척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
