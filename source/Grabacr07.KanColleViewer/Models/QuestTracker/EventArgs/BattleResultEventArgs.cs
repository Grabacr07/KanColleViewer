using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleViewer.Models.QuestTracker.Model;
using Grabacr07.KanColleViewer.Models.QuestTracker.Raw;

using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.EventArgs
{
    internal class BattleResultEventArgs
    {
        public string EnemyName { get; set; }
        public TrackerEnemyShip[] EnemyShips { get; set; }

        public int MapAreaId { get; set; }

        public bool IsFirstCombat { get; set; }
        public string Rank { get; set; }

        public BattleResultEventArgs(TrackerMapInfo MapInfo, TrackerEnemyShip[] enemyShips, kcsapi_battleresult data)
        {
            IsFirstCombat = MapInfo.IsFirstCombat;
            MapAreaId = MapInfo.MapId;
            EnemyName = data.api_enemy_info.api_deck_name;
            EnemyShips = enemyShips;
            Rank = data.api_win_rank;
        }
        public BattleResultEventArgs(TrackerMapInfo MapInfo, TrackerEnemyShip[] enemyShips, kcsapi_combined_battle_battleresult data)
        {
            IsFirstCombat = MapInfo.IsFirstCombat;
            MapAreaId = MapInfo.MapId;
            EnemyName = data.api_enemy_info.api_deck_name;
            EnemyShips = enemyShips;
            Rank = data.api_win_rank;
        }
    }
}
