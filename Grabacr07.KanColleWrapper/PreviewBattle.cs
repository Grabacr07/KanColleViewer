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
	/// <summary>
	/// 전투결과를 미리 받아 그것을 연산합니다.
	/// </summary>
	public class PreviewBattle
	{
		/// <summary>
		/// 언젠가 전투 미리보기가 실장될 경우를 대비한 부분.
		/// </summary>
		public bool EnablePreviewBattle { get; set; }

		/// <summary>
		/// 내부에서 크리티컬이 맞는지 조회하는 부분
		/// </summary>
		private bool IsCritical { get; set; }
		/// <summary>
		/// 팝업을 한번만 뜨도록 하기위해 존재하는 bool값
		/// </summary>
		public bool EventChecker { get; set; }

		public delegate void CriticalEventHandler();
		/// <summary>
		/// 크리티컬 컨디션 이벤트
		/// </summary>
		public event CriticalEventHandler CriticalCondition;
		/// <summary>
		/// 크리티컬 컨디션을 더이상 적용시키지 않기 위해 사용
		/// </summary>
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
		/// <summary>
		/// 대파 메시지가 한번만 뜨도록 조정하는걸 도와주는 부분. 현재 새로바뀐 구조에선 필요없을지도?
		/// </summary>
		/// <param name="cleared"></param>
		private void Cleared(kcsapi_battleresult cleared)
		{
			if (IsCritical) this.CriticalCleared();
		}

		/// <summary>
		/// battleresult창이 떴을때 IsCritical이 True이면 CriticalCondition이벤트를 발생
		/// </summary>
		/// <param name="results"></param>
		private void Result(kcsapi_battleresult results)
		{
			//if (IsCritical) MessageBox.Show("대파!");
			if (IsCritical) this.CriticalCondition();
		}
		/// <summary>
		/// 일반 포격전, 개막뇌격, 개막 항공전등을 계산. 현재 조금 분할해야할 필요성을 느낌.
		/// </summary>
		/// <param name="battle"></param>
		private void Battle(kcsapi_battle battle)
		{
			int[] Maxhps = battle.api_maxhps;
			int[] Nowhps = battle.api_nowhps;
			IsCritical = false;
			List<listup> lists = new List<listup>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();
			List<int> CurrentHPList = new List<int>();
			//모든 형태의 전투에서 타게팅이 되는 함선의 번호와 데미지를 순서대로 입력한다.

			//포격전 시작
			if (battle.api_hougeki1 != null)
				Listmake(battle.api_hougeki1.api_df_list, battle.api_hougeki1.api_damage, lists);
			if (battle.api_hougeki2 != null)
				Listmake(battle.api_hougeki2.api_df_list, battle.api_hougeki2.api_damage, lists);
			if (battle.api_hougeki3 != null)
				Listmake(battle.api_hougeki3.api_df_list, battle.api_hougeki3.api_damage, lists);
			//포격전 끝

			//뇌격전 시작
			if (battle.api_raigeki != null)
			{
				int[] numlist = battle.api_raigeki.api_erai;
				decimal[] damlist = battle.api_raigeki.api_eydam;
				gyoListmake(numlist, damlist, lists);

			}
			//뇌격전 끝

			//항공전 시작. 폭장과 뇌격 데미지는 합산되어있고 플래그로 맞는 녀석을 선별. 
			if (battle.api_kouku != null && battle.api_kouku.api_stage3 != null)
			{
				int[] numlist = battle.api_kouku.api_stage3.api_frai_flag;
				int[] numlistBak = battle.api_kouku.api_stage3.api_fbak_flag;
				decimal[] damlist = battle.api_kouku.api_stage3.api_fdam;
				for (int i = 0; i < battle.api_kouku.api_stage3.api_frai_flag.Count(); i++)
				{
					if (battle.api_kouku.api_stage3.api_fbak_flag[i]==1 || battle.api_kouku.api_stage3.api_frai_flag[i] == 1) numlist[i] = i;
				}
				gyoListmake(numlist, damlist, lists);
			}
			//항공전 끝

			//개막전 시작
			if (battle.api_opening_atack != null)
			{
				int[] numlist = battle.api_opening_atack.api_erai;
				decimal[] damlist = battle.api_opening_atack.api_eydam;
				gyoListmake(numlist, damlist, lists);
			}
			//개막전 끝

			//총 HP와 현재 HP의 리스트를 작성한다. 빈칸과 적은 제외. 여기서 적까지 포함시키고 별도의 함수를 추가하면 전투 미리보기 구현도 가능
			for (int i = 0; i < 7; i++)
			{
				if (Maxhps[i] != -1) MHPList.Add(Maxhps[i]);
				if (Nowhps[i] != -1) HPList.Add(Nowhps[i]);
			}
			//만들어진 데미지 리스트와 HP리스트를 총계내어서 현재 HP를 갱신한다.
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

			//현재 HP가 25%이하인지 판별한다.
			//지금 기능은 25퍼센트 이하인경우 리스트에 true값을 넣고 아니면 false를 넣는다.
			for (int i = 0; i < CurrentHPList.Count; i++)
			{
				double temp = (double)CurrentHPList[i] / (double)Maxhps[i + 1];
				if (temp <= 0.25) result.Add(true);
				else result.Add(false);
			}
			//아군이 아닌경우 모두 false를 집어넣는다. 전투 미리보기를 구현한다면 수정을 해야하는 부분
			for (int i = CurrentHPList.Count + 1; i < Maxhps.Count(); i++)
			{
				result.Add(false);
			}
			//result가 true인 경우 IsCritical과 EventChecker를 True로 한다.
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
		/// <summary>
		/// battle의 야전버전. Battle과 구조는 동일. 다만 전투의 양상이 조금 다르기때문에 분리
		/// </summary>
		/// <param name="battle"></param>
		private void MidBattle(kcsapi_midnight_battle battle)
		{
			int[] Maxhps = battle.api_maxhps;
			int[] Nowhps = battle.api_nowhps;
			IsCritical = false;
			List<listup> lists = new List<listup>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();
			List<int> CurrentHPList = new List<int>();
			//포격전 리스트를 작성. 주간과 달리 1차 포격전밖에 없음.
			if (battle.api_hougeki != null)
				Listmake(battle.api_hougeki.api_df_list, battle.api_hougeki.api_damage, lists);
			//HP리스트를 작성
			for (int i = 0; i < 7; i++)
			{
				if (Maxhps[i] != -1) MHPList.Add(Maxhps[i]);
				if (Nowhps[i] != -1) HPList.Add(Nowhps[i]);
			}
			//데미지를 계산하여 현재 HP에 적용
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
			//25%보다 적은지 많은지 판별
			List<bool> result = new List<bool>();
			for (int i = 0; i < CurrentHPList.Count; i++)
			{
				double temp = (double)CurrentHPList[i] / (double)Maxhps[i + 1];
				if (temp <= 0.25) result.Add(true);
				else result.Add(false);
			}
			//빈칸과 적칸에 모두 false를 입력
			for (int i = CurrentHPList.Count + 1; i < Maxhps.Count(); i++)
			{
				result.Add(false);
			}
			//리스트 전체에 true가 있는지 확인
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


		//리스트 작성부분은 좀 더 매끄러운 방법이 있는 것 같기도 하지만 일단 능력이 여기까지이므로
		/// <summary>
		/// 어뢰 데미지 리스트를 생성. 포격전이나 기타 컷인전투와 달리 Decimal값으로 바로 나온다.
		/// </summary>
		/// <param name="ho_target">어뢰 타격대상 리스트를 입력합니다.</param>
		/// <param name="ho_damage">어뢰 데미지를 입력합니다.</param>
		/// <param name="list">출력할 데미지 리스트를 입력합니다.</param>
		private void gyoListmake(int[] ho_target, decimal[] ho_damage, List<listup> list)
		{
			for (int i = 1; i < ho_target.Count(); i++)//-1제외
			{
				listup d = new listup();
				d.Num = ho_target[i];
				d.Damage = ho_damage[i];
				if (d.Num != 0) list.Add(d);
			}

		}
		/// <summary>
		/// 일반 포격 데미지 리스트를 작성합니다. object로 포장되어 있어 이걸 분해하여 리스트로 넣어줍니다.
		/// </summary>
		/// <param name="ho_target">타겟 리스트</param>
		/// <param name="ho_damage">데미지 리스트. 한 리스트에 object로 0~2까지 포장되어있음. 보통은 1까지만 쓰이는 모양</param>
		/// <param name="list"></param>
		private void Listmake(object[] ho_target, object[] ho_damage, List<listup> list)
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
