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
	/// 신편 제8구축대를 재편성하라!
	/// </summary>
	internal class A70 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 175;
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
					463, // 朝潮改二
					468, // 朝潮改二丁
				};
				var shipTable = new int[]
				{
					463, // 朝潮改二
					468, // 朝潮改二丁
					96,  // 大潮
					97,  // 満潮
					98,  // 荒潮
					249, // 大潮改
					250, // 満潮改
					251, // 荒潮改
					199, // 大潮改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					if (!flagshipTable.Contains((ships[0]?.Info.Id ?? 0))) continue; // 아사시오改2 기함

					count = Math.Max(
						count,
						ships.Length != 4 ? 0 : ships.Count(x => shipTable.Contains(x.Info.Id))
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
			return count >= max_count ? "완료" : "아사시오改2,오오시오,미치시오,아라시오 만으로 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
