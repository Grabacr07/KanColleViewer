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
	/// 신형 전탐을 배치하라!
	/// </summary>
	internal class A38 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 140;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;

				var homeport = KanColleClient.Current.Homeport;
				var flagship = homeport.Organization.Fleets[0].Ships[0];

				// 妙高改二
				count = flagship.Info.Id == 319 ? 1 : 0;

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
			return count >= max_count ? "완료" : "1함대 묘코改2 기함 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
