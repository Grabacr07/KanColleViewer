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
	/// 신형어뢰장비의 개발
	/// </summary>
	internal class F36 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 2;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 639;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			EventHandler handler = (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

				var flagshipTable = new int[]
				{
					50,  // 島風
					229, // 島風改
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets[0];
				if (!flagshipTable.Any(x => (x == (fleet?.Ships[0]?.Info.Id ?? 0)))) return; // 시마카제 기함

				var slotitems = fleet?.Ships[0]?.Slots;
				var cnt = 0;

				if (slotitems.Any(x => x.Item.Info.Id == 58 && x.Item.Level == 10)) cnt++;
				if (slotitems.Any(x => x.Item.Info.Id == 125 && x.Item.Level == 10)) cnt++;
				// 개수max 3연산, 개수max 5연산

				count = cnt;
				ProcessChanged?.Invoke(this, emptyEventArgs);
			};
			manager.HenseiEvent += handler;
			manager.EquipEvent += handler;
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
			return count >= max_count ? "완료" : "시마카제 기함, 숙련도max 61cm 3연장(산소)어뢰와 숙련도max 61cm 5연장(산소)어뢰 장비, 훈장 2개 소지 " + count.ToString() + " / " + max_count.ToString();
		}
	}
}
