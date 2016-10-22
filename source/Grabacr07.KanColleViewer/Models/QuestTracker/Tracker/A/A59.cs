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
	/// 제3항공전대를 편성하라!
	/// </summary>
	internal class A59 : NoSerializeTracker, ITracker
	{
		private readonly int max_count = 4;
		private int count;

		public event EventHandler ProcessChanged;

		int ITracker.Id => 164;
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
					112, // 瑞鶴改
					462, // 瑞鶴改二
					467, // 瑞鶴改二甲
				};
				var shipTable = new int[]
				{
					112, // 瑞鶴改
					462, // 瑞鶴改二
					467, // 瑞鶴改二甲
					116, // 瑞鳳
					102, // 千歳
					103, // 千代田
					117, // 瑞鳳改
					104, // 千歳改
					105, // 千代田改
					106, // 千歳甲
					107, // 千代田甲
					108, // 千歳航
					109, // 千代田航
					291, // 千歳航改
					292, // 千代田航改
					296, // 千歳航改二
					297, // 千代田航改二
				};

				var homeport = KanColleClient.Current.Homeport;
				foreach (var fleet in homeport.Organization.Fleets)
				{
					var ships = fleet.Value.Ships;
					if (ships.Length <= 0) continue;

					if (!flagshipTable.Contains((ships[0]?.Info.Id ?? 0))) continue; // 즈이카쿠改 기함

					count = Math.Max(
						count,
						ships.Count(x => shipTable.Contains(x.Id))
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
			return count >= max_count ? "완료" : "즈이카쿠改 기함,즈이호,치토세,치요다 편성 (" + count.ToString() + " / " + max_count.ToString() + ")";
		}
	}
}
