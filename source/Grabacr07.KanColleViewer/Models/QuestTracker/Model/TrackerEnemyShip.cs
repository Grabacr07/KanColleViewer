using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Model
{
    class TrackerEnemyShip
    {
        public int Id { get; set; }

        public int MaxHp { get; set; }
        public int NowHp { get; set; }

        public int Type { get; set; }
    }
}
