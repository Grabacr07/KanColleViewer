using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Model
{
    class TrackerMapInfo
    {
        public bool IsFirstCombat { get; set; }
        public int MapId { get; set; }

        public TrackerMapInfo AfterCombat()
        {
            var prev = new TrackerMapInfo
            {
                IsFirstCombat = this.IsFirstCombat,
                MapId = this.MapId
            };
            this.IsFirstCombat = false;
            return prev;
        }
        public void Reset(int MapId)
        {
            this.IsFirstCombat = true;
            this.MapId = MapId;
        }
    }
}
