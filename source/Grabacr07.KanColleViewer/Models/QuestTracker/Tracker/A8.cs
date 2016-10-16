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
	/// 텐류급 경순자매 전 2척을 편성하라
	/// </summary>
	internal class A8 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 2;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 108;
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
					51,  // 天龍
					52,  // 龍田
					213, // 天龍改
					214  // 龍田改
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					count = Math.Max(
						count,
						fleet.Value.Ships.Count(x => shipTable.Contains(x.Info.Id)).Max(2)
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
			return count >= max_count ? "완료" : "텐류, 타츠타 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
