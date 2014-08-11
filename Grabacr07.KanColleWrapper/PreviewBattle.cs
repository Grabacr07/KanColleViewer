using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;
using System.Windows;

namespace Grabacr07.KanColleWrapper.Models
{
	public class PreviewBattle
	{
		public bool EnablePreviewBattle { get; set; }
		public bool IsCritical { get; set; }
		public bool EventChecker { get; set; }

		public delegate void CriticalEventHandler();

		public event CriticalEventHandler CriticalCondition;
		public event CriticalEventHandler CriticalCleared;
		public PreviewBattle(KanColleProxy proxy)
		{
			proxy.api_req_sortie_battle.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(x.Data));
			proxy.api_req_sortie_night_to_day.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(x.Data));
			proxy.api_req_battle_midnight_battle.TryParse<kcsapi_midnight_battle>().Subscribe(x => this.MidBattle(x.Data));
			proxy.api_req_battle_midnight_sp_midnight.TryParse<kcsapi_midnight_battle>().Subscribe(x => this.MidBattle(x.Data));
			proxy.api_req_sortie_battleresult.TryParse<kcsapi_battleresult>().Subscribe(x => this.Result(x.Data));
			proxy.api_port.TryParse<kcsapi_battleresult>().Subscribe(x => this.Cleared(x.Data));
			//proxy.api_req_combined_battle_battle.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(x.Data));
			//proxy.api_req_combined_battle.TryParse<kcsapi_battleresult>().Subscribe(x => this.Result(x.Data));
			//"/kcsapi/api_req_combined_battle/airbattle",

		}
		private void Cleared(kcsapi_battleresult cleared)
		{
			if (IsCritical) this.CriticalCleared();
		}
		private void Result(kcsapi_battleresult results)
		{
			//if (IsCritical) MessageBox.Show("대파!");
			if (IsCritical) this.CriticalCondition();
		}
		private void Battle(kcsapi_battle battle)
		{
			int[] Maxhps = battle.api_maxhps;
			int[] Nowhps = battle.api_nowhps;
			IsCritical = false;
			List<listup> lists = new List<listup>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();
			List<int> CurrentHPList = new List<int>();

			if (battle.api_hougeki1 != null)
				Listmake(battle.api_hougeki1.api_df_list, battle.api_hougeki1.api_damage, lists);
			if (battle.api_hougeki2 != null)
				Listmake(battle.api_hougeki2.api_df_list, battle.api_hougeki2.api_damage, lists);
			if (battle.api_hougeki3 != null)
				Listmake(battle.api_hougeki3.api_df_list, battle.api_hougeki3.api_damage, lists);
			if (battle.api_raigeki != null)
			{
				int[] numlist = battle.api_raigeki.api_erai;
				decimal[] damlist = battle.api_raigeki.api_eydam;
				gyoListmake(numlist, damlist, lists);

			}
			if (battle.api_kouku != null && battle.api_kouku.api_stage3 != null)
			{
				int[] numlist = battle.api_kouku.api_stage3.api_frai_flag;
				decimal[] damlist = battle.api_kouku.api_stage3.api_fdam;
				for (int i = 0; i < battle.api_kouku.api_stage3.api_frai_flag.Count(); i++)
				{
					if (battle.api_kouku.api_stage3.api_frai_flag[i] == 1) numlist[i] = i;
				}
				gyoListmake(numlist, damlist, lists);
			}
			if (battle.api_opening_atack != null)
			{
				int[] numlist = battle.api_opening_atack.api_erai;
				decimal[] damlist = battle.api_opening_atack.api_eydam;
				gyoListmake(numlist, damlist, lists);
			}
			for (int i = 0; i < 7; i++)//총 HP. -1, 적 제외
			{
				if (Maxhps[i] != -1) MHPList.Add(Maxhps[i]);
				if (Nowhps[i] != -1) HPList.Add(Nowhps[i]);
			}
			for (int i = 0; i < HPList.Count; i++)
			{
				int MaxHP = MHPList[i];
				int CurrentHP = HPList[i];

				for (int j = 0; j < lists.Count; j++)
				{
					decimal rounded = decimal.Round(lists[j].Damage);
					int damage = (int)rounded;
					if (lists[j].Num == i + 1) CurrentHP = CurrentHP - damage;
				}
				CurrentHPList.Add(CurrentHP);
			}
			List<bool> result = new List<bool>();
			for (int i = 0; i < CurrentHPList.Count; i++)
			{
				double temp = (double)CurrentHPList[i] / (double)Maxhps[i + 1];
				if (temp <= 0.25) result.Add(true);
				else result.Add(false);
			}
			for (int i = CurrentHPList.Count + 1; i < Maxhps.Count(); i++)
			{
				result.Add(false);
			}
			foreach (bool t in result)
			{
				if (t)
				{
					IsCritical = true;
					EventChecker = true;
					break;
				}
			}
		}
		private void MidBattle(kcsapi_midnight_battle battle)
		{
			int[] Maxhps = battle.api_maxhps;
			int[] Nowhps = battle.api_nowhps;
			IsCritical = false;
			List<listup> lists = new List<listup>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();
			List<int> CurrentHPList = new List<int>();

			if (battle.api_hougeki != null)
				Listmake(battle.api_hougeki.api_df_list, battle.api_hougeki.api_damage, lists);
			for (int i = 0; i < 7; i++)//총 HP. -1, 적 제외
			{
				if (Maxhps[i] != -1) MHPList.Add(Maxhps[i]);
				if (Nowhps[i] != -1) HPList.Add(Nowhps[i]);
			}
			for (int i = 0; i < HPList.Count; i++)
			{
				int MaxHP = MHPList[i];
				int CurrentHP = HPList[i];

				for (int j = 0; j < lists.Count; j++)
				{
					decimal rounded = decimal.Round(lists[j].Damage);
					int damage = (int)rounded;
					if (lists[j].Num == i + 1) CurrentHP = CurrentHP - damage;
				}
				CurrentHPList.Add(CurrentHP);
			}
			List<bool> result = new List<bool>();
			for (int i = 0; i < CurrentHPList.Count; i++)
			{
				double temp = (double)CurrentHPList[i] / (double)Maxhps[i + 1];
				if (temp <= 0.25) result.Add(true);
				else result.Add(false);
			}
			for (int i = CurrentHPList.Count + 1; i < Maxhps.Count(); i++)
			{
				result.Add(false);
			}
			foreach (bool t in result)
			{
				if (t)
				{
					IsCritical = true;
					EventChecker = true;
					break;
				}
			}
		}

		public void gyoListmake(int[] ho_target, decimal[] ho_damage, List<listup> list)
		{
			for (int i = 1; i < ho_target.Count(); i++)//-1제외
			{
				listup d = new listup();
				d.Num = ho_target[i];
				d.Damage = ho_damage[i];
				if (d.Num != 0) list.Add(d);
			}

		}
		public void Listmake(object[] ho_target, object[] ho_damage, List<listup> list)
		{
			for (int i = 1; i < ho_target.Count(); i++)//-1제외
			{
				listup d = new listup();
				dynamic listNum = ho_target[i];
				dynamic damNum = ho_damage[i];
				d.Num = listNum[0];
				d.Damage = damNum[0];

				if (damNum.Length > 1)
				{
					if (damNum[1] > 0) d.Damage = damNum[0] + damNum[1];
				}
				list.Add(d);
			}

		}


	}
}
