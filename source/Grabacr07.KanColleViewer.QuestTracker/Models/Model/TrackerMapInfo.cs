using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Model
{
	class TrackerMapInfo
	{
		public bool IsFirstCombat { get; set; }
		public int WorldId { get; set; }
		public int MapId { get; set; }
		public int NodeId { get; set; }
		public bool IsBoss { get; set; }

		public TrackerMapInfo AfterCombat()
		{
			var prev = new TrackerMapInfo
			{
				IsFirstCombat = this.IsFirstCombat,
				WorldId = this.WorldId,
				MapId = this.MapId,
				NodeId = this.NodeId,
				IsBoss = this.IsBoss
			};
			this.IsFirstCombat = false;
			return prev;
		}
		public void Reset(int WorldId, int MapId, int NodeId, bool IsBoss)
		{
			this.IsFirstCombat = true;
			this.WorldId = WorldId;
			this.MapId = MapId;
			this.NodeId = NodeId;
			this.IsBoss = IsBoss;
		}
		public void Next(int WorldId, int MapId, int NodeId, bool IsBoss)
		{
			this.WorldId = WorldId;
			this.MapId = MapId;
			this.NodeId = NodeId;
			this.IsBoss = IsBoss;
		}
	}
}
