﻿using System;
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
	/// 항모기동부대를 편성하라!
	/// </summary>
	internal class A12 : NoSerializeOverUnderTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 107;
		public QuestType Type => QuestType.OneTime;
		public bool IsTracking { get; set; }

		private System.EventArgs emptyEventArgs = new System.EventArgs();

		public void RegisterEvent(TrackManager manager)
		{
			manager.HenseiEvent += (sender, args) =>
			{
				if (!IsTracking) return;
				count = 0;

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					var flagship = ships[0]?.Info.ShipType.Id;

					// 기함 공모/장갑공모
					count = Math.Max(
						count,
						(flagship != 7 && flagship != 11 && flagship != 18)
							? 0
							: ships.Count(x => x.Info.ShipType.Id == 7 || x.Info.ShipType.Id == 11 || x.Info.ShipType.Id == 18).Max(1)
								+ ships.Count(x => x.Info.ShipType.Id == 2).Max(3)
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
			return count >= max_count ? "완료" : "공모 기함 1척,구축 3척 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
