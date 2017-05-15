using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models.Raw;

using Grabacr07.KanColleViewer.QuestTracker.Models.Extensions;

namespace Grabacr07.KanColleViewer.QuestTracker.Models.Model
{
	class BattleTracker
	{
		public TrackerEnemyShip[] enemyShips;
		public TrackerEnemyShip[] enemyShips2;

		public BattleTracker()
		{
			enemyShips = new TrackerEnemyShip[]
			{
				new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip(),
				new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip()
			};
			enemyShips2 = new TrackerEnemyShip[]
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
				data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
				data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
				data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
				data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike
				data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
				data.api_opening_taisen.GetEnemyDamages(),              // Opening ASW
				data.api_opening_atack.GetEnemyDamages(),               // Opening Torpedo
				data.api_hougeki1.GetEnemyDamages(),                    // Duel 1
				data.api_hougeki2.GetEnemyDamages(),                    // Duel 2
				data.api_raigeki.GetEnemyDamages()                      // Torpedo
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
		public void BattleProcess(kcsapi_sortie_airbattle data)
		{
			ResetEnemy(data.api_ship_ke);

			UpdateEnemyMaxHP(data.api_maxhps);
			UpdateEnemyNowHP(data.api_nowhps);

			CalcEnemyDamages(
				data.api_air_base_injection.GetEnemyDamages(),  // AirBase Jet
				data.api_injection_kouku.GetEnemyDamages(),     // Fleet Jet
				data.api_air_base_attack.GetEnemyDamages(),     // AirBase Airstrike

				data.api_kouku.GetEnemyDamages(),               // Fleet Airstrike
				data.api_kouku2.GetEnemyDamages()               // Fleet 2nd Airstrike
			);
		} // 항공전

		public void BattleProcess(kcsapi_combined_battle data) // 연합함대 (기동,수송,수상)
		{
			ResetEnemy(data.api_ship_ke);

			UpdateEnemyMaxHP(data.api_maxhps);
			UpdateEnemyNowHP(data.api_nowhps);

			CalcEnemyDamages(
				data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
				data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
				data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
				data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike
				data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
				data.api_opening_taisen.GetEnemyDamages(),              // Opening ASW
				data.api_opening_atack.GetEnemyDamages(),               // Opening Torpedo
				data.api_hougeki1.GetEnemyDamages(),                    // Duel 1
				data.api_hougeki2.GetEnemyDamages(),                    // Duel 2
				data.api_hougeki3.GetEnemyDamages(),                    // Duel 3
				data.api_raigeki.GetEnemyDamages()                      // Torpedo
			);
		}
		public void BattleProcess(kcsapi_combined_battle_airbattle data) // 연합함대 항공전
		{
			ResetEnemy(data.api_ship_ke);

			UpdateEnemyMaxHP(data.api_maxhps);
			UpdateEnemyNowHP(data.api_nowhps);

			CalcEnemyDamages(
				data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
				data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
				data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
				data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike

				data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
				data.api_kouku.GetEnemyDamages()                        // Fleet 2nd Airstrike
			);
		}
		public void BattleProcess(kcsapi_combined_battle_midnight_battle data) // 연합함대 야전
		{
			UpdateEnemyNowHP(data.api_nowhps);

			CalcEnemyDamages(data.api_hougeki.GetEnemyDamages());
		}

		public void BattleProcess(kcsapi_combined_each_battle data, bool isCombined) // vs심해연합
		{
			ResetEnemy(data.api_ship_ke, data.api_ship_ke_combined);

			UpdateEnemyMaxHP(data.api_maxhps, data.api_maxhps_combined);
			UpdateEnemyNowHP(data.api_nowhps, data.api_nowhps_combined);

			#region 연합vs연합
			if (isCombined)
			{
				CalcEnemyDamages(
					data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
					data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
					data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
					data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike
					data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
					data.api_opening_taisen.GetEnemyDamages(),              // Opening ASW
					data.api_opening_atack.GetEnemyDamages(),               // Opening Torpedo
					data.api_hougeki1.GetEnemyDamages(),                    // Duel 1
					data.api_raigeki.GetEnemyDamages(),                     // Torpedo
					data.api_hougeki3.GetEnemyDamages()                     // Duel 3
				);
				CalcEnemyDamages2(
					data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
					data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
					data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
					data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike
					data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
					data.api_opening_taisen.GetEnemyDamages(),              // Opening ASW
					data.api_opening_atack.GetEnemyDamages(),               // Opening Torpedo
					data.api_hougeki2.GetEnemyDamages(),                    // Duel 2
					data.api_raigeki.GetEnemyDamages(),                     // Torpedo
					data.api_hougeki3.GetEnemyDamages()                     // Duel 3
				);
			}
			#endregion

			#region 단일vs연합
			else
			{
				CalcEnemyDamages(
					data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
					data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
					data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
					data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike
					data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
					data.api_opening_taisen.GetEnemyDamages(),              // Opening ASW
					data.api_opening_atack.GetEnemyDamages(),               // Opening Torpedo
					data.api_raigeki.GetEnemyDamages(),                     // Torpedo
					data.api_hougeki2.GetEnemyDamages(),                    // Duel 2
					data.api_hougeki3.GetEnemyDamages()                     // Duel 3
				);

				CalcEnemyDamages2(
					data.api_air_base_injection.GetEnemyDamages(),          // AirBase Jet
					data.api_injection_kouku.GetEnemyDamages(),             // Fleet Jet
					data.api_air_base_attack.GetEachFirstEnemyDamages(),    // AirBase Airstrike
					data.api_support_info.GetEnemyDamages(),                // Support-fleet Firestrike
					data.api_kouku.GetEnemyDamages(),                       // Fleet Airstrike
					data.api_opening_taisen.GetEnemyDamages(),              // Opening ASW
					data.api_opening_atack.GetEnemyDamages(),               // Opening Torpedo
					data.api_raigeki.GetEnemyDamages(),                     // Torpedo
					data.api_hougeki1.GetEnemyDamages(),                    // Duel 1
					data.api_hougeki3.GetEnemyDamages()                     // Duel 3
				);
			}
			#endregion
		}
		public void BattleProcess(kcsapi_combined_each_midnight_battle data) // vs심해연합 야전
		{
			UpdateEnemyNowHP(data.api_nowhps, data.api_nowhps_combined);

			if (data.api_active_deck[1] == 1)
				CalcEnemyDamages(data.api_hougeki.GetEnemyDamages());
			else
				CalcEnemyDamages2(data.api_hougeki.GetEnemyDamages());
		}

		public void ResetEnemy(int[] api_ship_ke, int[] api_ship_ke2 = null)
		{
			enemyShips = new TrackerEnemyShip[]
			{
				new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip(),
				new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip()
			};
			enemyShips2 = new TrackerEnemyShip[]
			{
				new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip(),
				new TrackerEnemyShip(), new TrackerEnemyShip(), new TrackerEnemyShip()
			};

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

			if (api_ship_ke2 != null)
			{
				api_ship_ke2 = new int[6]
				{
					api_ship_ke2[1], api_ship_ke2[2], api_ship_ke2[3],
					api_ship_ke2[4], api_ship_ke2[5], api_ship_ke2[6]
				};
				for (var i = 0; i < api_ship_ke2.Length; i++)
				{
					var id = api_ship_ke2[i];
					enemyShips2[i].Id = id;
					enemyShips2[i].Type = id == -1 ? 0 : KanColleClient.Current.Master.Ships[id].ShipType.Id;
				}
			}
		}

		public void UpdateEnemyMaxHP(int[] api_maxhps, int[] api_maxhps2 = null)
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

			if (api_maxhps2 != null)
			{
				api_maxhps2 = new int[6]
				{
					api_maxhps2[7], api_maxhps2[8], api_maxhps2[9],
					api_maxhps2[10], api_maxhps2[11], api_maxhps2[12]
				};
				for (var i = 0; i < api_maxhps2.Length; i++)
				{
					var hp = api_maxhps2[i] == -1 ? int.MaxValue : api_maxhps2[i];
					enemyShips2[i].MaxHp = hp;
				}
			}
		}
		public void UpdateEnemyNowHP(int[] api_nowhps, int[] api_nowhps2 = null)
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

			if (api_nowhps2 != null)
			{
				api_nowhps2 = new int[6]
				{
					api_nowhps2[7], api_nowhps2[8], api_nowhps2[9],
					api_nowhps2[10], api_nowhps2[11], api_nowhps2[12]
				};
				for (var i = 0; i < api_nowhps2.Length; i++)
				{
					var hp = api_nowhps2[i] == -1 ? int.MaxValue : api_nowhps2[i];
					enemyShips2[i].NowHp = hp;
				}
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
		public void CalcEnemyDamages2(params FleetDamages[] damages)
		{
			foreach (var damage in damages)
			{
				enemyShips2[0].NowHp -= damage.Ship1;
				enemyShips2[1].NowHp -= damage.Ship2;
				enemyShips2[2].NowHp -= damage.Ship3;
				enemyShips2[3].NowHp -= damage.Ship4;
				enemyShips2[4].NowHp -= damage.Ship5;
				enemyShips2[5].NowHp -= damage.Ship6;
			}
		}
	}
}
