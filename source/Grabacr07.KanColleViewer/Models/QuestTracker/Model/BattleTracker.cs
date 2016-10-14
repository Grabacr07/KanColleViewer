using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

using Grabacr07.KanColleViewer.Models.QuestTracker.Extensions;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Model
{
    class BattleTracker
    {
        public TrackerEnemyShip[] enemyShips;

        public BattleTracker()
        {
            enemyShips = new TrackerEnemyShip[]
            {
                new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip(),
                new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip()
            };
        }

        public void BattleProcess(kcsapi_sortie_battle data)
        {
            ResetEnemy(data.api_ship_ke);

            UpdateEnemyMaxHP(data.api_maxhps);
            UpdateEnemyNowHP(data.api_nowhps);

            CalcEnemyDamages(
                data.api_air_base_attack.GetEnemyDamages(), // 기항대
                data.api_support_info.GetEnemyDamages(),    // 지원함대
                data.api_kouku.GetEnemyDamages(),           // 항공전
                data.api_opening_taisen.GetEnemyDamages(),  // 선제대잠
                data.api_opening_atack.GetEnemyDamages(),   // 선제뇌격
                data.api_hougeki1.GetEnemyDamages(),        // 1차 포격전
                data.api_hougeki2.GetEnemyDamages(),        // 2차 포격전
                data.api_raigeki.GetEnemyDamages()          // 뇌격전
            );
        }
        public void BattleProcess(kcsapi_battle_midnight_battle data) // 야전
        {
            UpdateEnemyNowHP(data.api_nowhps);

            CalcEnemyDamages(data.api_hougeki.GetEnemyDamages());
        }
        public void BattleProcess(kcsapi_battle_midnight_sp_midnight data) // 개막야전
        {
            ResetEnemy(data.api_ship_ke);

            UpdateEnemyMaxHP(data.api_maxhps);
            UpdateEnemyNowHP(data.api_nowhps);

            CalcEnemyDamages(data.api_hougeki.GetEnemyDamages());
        }

        public void ResetEnemy(int[] api_ship_ke)
        {
            api_ship_ke = new int[6]
            {
                api_ship_ke[1], api_ship_ke[2], api_ship_ke[3],
                api_ship_ke[4], api_ship_ke[5], api_ship_ke[6]
            };

            for (var i = 0; i < api_ship_ke.Length; i++)
            {
                var id = api_ship_ke[i];

                enemyShips[i].Id = id;
                enemyShips[i].Type = id == -1 ? 0 : KanColleClient.Current.Master.Ships[id].ShipType.Id;
            }
        }

        public void UpdateEnemyMaxHP(int[] api_maxhps)
        {
            api_maxhps = new int[6]
            {
                api_maxhps[7], api_maxhps[8], api_maxhps[9],
                api_maxhps[10], api_maxhps[11], api_maxhps[12]
            };

            for (var i = 0; i < api_maxhps.Length; i++)
            {
                var hp = api_maxhps[i] == -1 ? int.MaxValue : api_maxhps[i];
                enemyShips[i].MaxHp = hp;
            }
        }

        public void UpdateEnemyNowHP(int[] api_nowhps)
        {
            api_nowhps = new int[6]
            {
                api_nowhps[7], api_nowhps[8], api_nowhps[9],
                api_nowhps[10], api_nowhps[11], api_nowhps[12]
            };

            for (var i = 0; i < api_nowhps.Length; i++)
            {
                var hp = api_nowhps[i] == -1 ? int.MaxValue : api_nowhps[i];
                enemyShips[i].NowHp = hp;
            }
        }

        public void CalcEnemyDamages(params FleetDamages[] damages)
        {
            foreach (var damage in damages)
            {
                enemyShips[0].NowHp -= damage.Ship1;
                enemyShips[1].NowHp -= damage.Ship2;
                enemyShips[2].NowHp -= damage.Ship3;
                enemyShips[3].NowHp -= damage.Ship4;
                enemyShips[4].NowHp -= damage.Ship5;
                enemyShips[5].NowHp -= damage.Ship6;
            }
        }
    }
}
