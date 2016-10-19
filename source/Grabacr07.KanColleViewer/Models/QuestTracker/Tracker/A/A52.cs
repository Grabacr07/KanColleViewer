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
	/// 해상돌입부대를 편성하라!
	/// </summary>
	internal class A52 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 6;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 154;
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
					86,  // 比叡
					85,  // 霧島
					21,  // 長良
					34,  // 暁
					36,  // 雷
					37,  // 電
					150, // 比叡改
					152, // 霧島改
					218, // 長良改
					234, // 暁改
					236, // 雷改
					237, // 電改
					210, // 比叡改二
					212, // 霧島改二
					437, // 暁改二
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
			return count >= max_count ? "완료" : "히에이,키리시마,나가라,아카츠키,이카즈치,이나즈마 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
