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
	/// 숙련승무원 양성
	/// </summary>
	internal class F35 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 1;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 637;
		public QuestType Type => QuestType.Other; // 계절
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
					89,  // 鳳翔
					285, // 鳳翔改
				};

				var fleet = KanColleClient.Current.Homeport.Organization.Fleets[0];
				if (!flagshipTable.Any(x => x == (fleet?.Ships[0]?.Info.Id ?? 0))) return; // 호쇼 기함

				var slotitems = fleet?.Ships[0]?.Slots;
				if (!slotitems.Any(x => x.Item.Info.Id == 19 && x.Item.Level == 10 && x.Item.Proficiency == 7)) return;
				// 숙련도max, 개수max 96식함전

				count = 1;
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
			return count >= max_count ? "완료" : "호쇼 기함, 숙련도max 개수max 96식 함상전투기 장착, 훈장 2개 소지 " + count.ToString() + " / " + max_count.ToString();
		}
	}
}
