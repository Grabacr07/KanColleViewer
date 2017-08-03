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
	/// 제5전대를 편성하라!
	/// </summary>
	internal class A35 : NoSerializeOverUnderTracker, ITracker
	{
		private readonly int max_count = 3;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 137;
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
					62,  // 妙高
					63,  // 那智
					65,  // 羽黒
					265, // 妙高改
					266, // 那智改
					268, // 羽黒改
					319, // 妙高改二
					192, // 那智改二
					194, // 羽黒改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;

					count = Math.Max(
						count,
						ships.Length != 3 ? 0 : ships.Count(x => shipTable.Contains(x.Info.Id))
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
			return count >= max_count ? "완료" : "묘코,나치,하구로 만으로 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
