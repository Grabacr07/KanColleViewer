using Grabacr07.KanColleWrapper.Models.Raw;
using System;
using System.Collections.Generic;
using System.Linq;

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
		protected bool EnablePreviewBattle { get; set; }
		/// <summary>
		/// 내부에서 크리티컬이 맞는지 조회하는 부분
		/// </summary>
		private bool IsCritical { get; set; }
		/// <summary>
		/// 전투가 끝나고 모항에 돌아왔는지를 채크
		/// </summary>
		private bool BattleEnd { get; set; }

		public delegate void CriticalEventHandler();
		/// <summary>
		/// 크리티컬 컨디션 이벤트
		/// </summary>
		public event CriticalEventHandler CriticalCondition;
		/// <summary>
		/// 크리티컬 컨디션을 더이상 적용시키지 않기 위해 사용
		/// </summary>
		public event CriticalEventHandler CriticalCleared;
		/// <summary>
		/// 전투결과를 미리 계산합니다. 현재는 대파알림 전용으로 만들어져있습니다.
		/// </summary>
		/// <param name="proxy"></param>
		public PreviewBattle(KanColleProxy proxy)
		{
			proxy.api_req_sortie_battle.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(false,false, x.Data));
			proxy.api_req_sortie_night_to_day.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(false,false, x.Data));
			proxy.api_req_battle_midnight_battle.TryParse<kcsapi_midnight_battle>().Subscribe(x => this.MidBattle(false, x.Data));
			proxy.api_req_battle_midnight_sp_midnight.TryParse<kcsapi_midnight_battle>().Subscribe(x => this.MidBattle(false, x.Data));
			proxy.api_req_combined_battle_airbattle.TryParse<kcsapi_battle>().Subscribe(x => this.AirBattle(x.Data));
			proxy.api_req_combined_battle_battle.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(true,false, x.Data));
			proxy.api_req_combined_battle_midnight_battle.TryParse<kcsapi_midnight_battle>().Subscribe(x => this.MidBattle(true, x.Data));
			proxy.api_req_combined_battle_battle_water.TryParse<kcsapi_battle>().Subscribe(x => this.Battle(true,true, x.Data));
			
			proxy.api_req_map_start.Subscribe(x => this.Cleared(false));

			proxy.api_req_sortie_battleresult.TryParse().Subscribe(x => this.Result());
			proxy.api_req_combined_battle_battleresult.TryParse().Subscribe(x => this.Result());
			proxy.api_port.TryParse().Subscribe(x => this.Cleared(true));
		}

		/// <summary>
		/// 회항하였을때 테마와 악센트를 원래대로
		/// </summary>
		/// <param name="IsEnd">전투가 끝난건지 안 끝난건지의 여부를 입력</param>
		private void Cleared(bool IsEnd)
		{
			if (this.IsCritical)
			{
				this.CriticalCleared();
				this.IsCritical = false;
				
				if (IsEnd) this.BattleEnd = true;
				else this.BattleEnd = false;
			}
		}
		/// <summary>
		/// battleresult창이 떴을때 IsCritical이 True이면 CriticalCondition이벤트를 발생
		/// </summary>
		/// <param name="results"></param>
		private void Result()
		{
			if (this.IsCritical) this.CriticalCondition();
		}
		/// <summary>
		/// 대파알림 계산이 잘못되었거나 문제가 생겼을때를 대비한 코드.
		/// </summary>
		public void AfterResult()
		{
			if (!this.IsCritical)
			{
				if (!this.BattleEnd)
				{
					this.CriticalCondition();
					this.IsCritical = true;
				}
			}
		}
		/// <summary>
		/// 연합함대를 사용한 전투에서 항공전을 처리하는데 사용.
		/// </summary>
		/// <param name="battle"></param>
		private void AirBattle(kcsapi_battle battle)
		{
			this.IsCritical = false;
			List<listup> lists = new List<listup>();
			List<int> CurrentHPList = new List<int>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();

			List<listup> Combinelists = new List<listup>();


			//api_kouku시작
			if (battle.api_kouku != null && battle.api_kouku.api_stage3 != null)
			{
				int[] numlist = battle.api_kouku.api_stage3.api_frai_flag;

				decimal[] damlist = battle.api_kouku.api_stage3.api_fdam;

				ChangeKoukuFlagToNumber(
					battle.api_kouku.api_stage3.api_frai_flag,
					battle.api_kouku.api_stage3.api_fbak_flag,
					numlist);
				DecimalListmake(numlist, damlist, lists);
				//리스트의 재활용. 여기부터 연합함대 리스트 작성
				if (battle.api_kouku != null && battle.api_kouku.api_stage3_combined != null)
				{
					numlist = battle.api_kouku.api_stage3_combined.api_frai_flag;

					damlist = battle.api_kouku.api_stage3_combined.api_fdam;

					ChangeKoukuFlagToNumber(
						battle.api_kouku.api_stage3_combined.api_frai_flag,
						battle.api_kouku.api_stage3_combined.api_fbak_flag,
						numlist);
					DecimalListmake(numlist, damlist, Combinelists);
				}//연합함대 리스트 작성 끝
			}
			//api_kouku끝

			//api_kouku2시작
			if (battle.api_kouku2 != null && battle.api_kouku2.api_stage3 != null)
			{
				int[] numlist = battle.api_kouku2.api_stage3.api_frai_flag;//뇌격
				int[] numlistBak = battle.api_kouku2.api_stage3.api_fbak_flag;//폭격

				decimal[] damlist = battle.api_kouku2.api_stage3.api_fdam;

				ChangeKoukuFlagToNumber(battle.api_kouku2.api_stage3.api_frai_flag,
					battle.api_kouku2.api_stage3.api_fbak_flag,
					numlist);
				DecimalListmake(numlist, damlist, lists);//메인 함대 리스트 작성 끝

				//리스트의 재활용. 여기부터 연합함대 리스트 작성
				if (battle.api_kouku2 != null && battle.api_kouku2.api_stage3_combined != null)
				{
					numlist = battle.api_kouku2.api_stage3_combined.api_frai_flag;//뇌격
					numlistBak = battle.api_kouku2.api_stage3_combined.api_fbak_flag;//폭격

					damlist = battle.api_kouku2.api_stage3_combined.api_fdam;
					for (int i = 0; i < battle.api_kouku2.api_stage3_combined.api_frai_flag.Count(); i++)//컴바인 함대 플래그
					{
						if (
							battle.api_kouku2.api_stage3_combined.api_fbak_flag[i] == 1 ||
							battle.api_kouku2.api_stage3_combined.api_frai_flag[i] == 1)

							numlist[i] = i;
					}
					DecimalListmake(numlist, damlist, Combinelists);
				}//연합함대 리스트 작성 끝
			}
			//api_kouku끝
			BattleCalc(HPList, MHPList, lists, CurrentHPList, battle.api_maxhps, battle.api_nowhps);

			//재활용 위해 초기화
			CurrentHPList = new List<int>();
			HPList = new List<int>();
			MHPList = new List<int>();

			BattleCalc(HPList, MHPList, Combinelists, CurrentHPList, battle.api_maxhps_combined, battle.api_nowhps_combined);
		}
		/// <summary>
		/// 일반 포격전, 개막뇌격, 개막 항공전등을 계산.
		/// </summary>
		/// <param name="battle"></param>
		/// <param name="IsCombined">연합함대인경우 True로 설정합니다.</param>
		/// <param name="IsWater">수뢰전대인지 채크합니다</param>
		private void Battle(bool IsCombined,bool IsWater, kcsapi_battle battle)
		{
			this.IsCritical = false;
			List<int> CurrentHPList = new List<int>();
			List<listup> lists = new List<listup>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();

			List<listup> Combinelists = new List<listup>();
			//모든 형태의 전투에서 타게팅이 되는 함선의 번호와 데미지를 순서대로 입력한다.
			//포격전 시작
			if (IsWater)
			{
				if (!IsCombined && battle.api_hougeki3 != null)
					ObjectListmake(battle.api_hougeki3.api_df_list, battle.api_hougeki3.api_damage, lists);
				else if (IsCombined && battle.api_hougeki3 != null)//1차 포격전은 2함대만 맞을지도...?
					ObjectListmake(battle.api_hougeki3.api_df_list, battle.api_hougeki3.api_damage, Combinelists);
				if (battle.api_hougeki1 != null)
					ObjectListmake(battle.api_hougeki1.api_df_list, battle.api_hougeki1.api_damage, lists);
			}
			else
			{
				if (!IsCombined && battle.api_hougeki1 != null)
					ObjectListmake(battle.api_hougeki1.api_df_list, battle.api_hougeki1.api_damage, lists);
				else if (IsCombined && battle.api_hougeki1 != null)//1차 포격전은 2함대만 맞을지도...?
					ObjectListmake(battle.api_hougeki1.api_df_list, battle.api_hougeki1.api_damage, Combinelists);
				if (battle.api_hougeki3 != null)
					ObjectListmake(battle.api_hougeki3.api_df_list, battle.api_hougeki3.api_damage, lists);
			}
			if (battle.api_hougeki2 != null)
				ObjectListmake(battle.api_hougeki2.api_df_list, battle.api_hougeki2.api_damage, lists);
			
			//포격전 끝

			//뇌격전 시작
			if (battle.api_raigeki != null)
			{
				int[] numlist = battle.api_raigeki.api_erai;
				decimal[] damlist = battle.api_raigeki.api_eydam;
				if (!IsCombined) 
					DecimalListmake(numlist, damlist, lists);
				else if (IsCombined)
					DecimalListmake(numlist, damlist, Combinelists);
			}
			//뇌격전 끝

			//항공전 시작. 폭장과 뇌격 데미지는 합산되어있고 플래그로 맞는 녀석을 선별. 
			if (battle.api_kouku != null && battle.api_kouku.api_stage3 != null)
			{
				int[] numlist = battle.api_kouku.api_stage3.api_frai_flag;//뇌격

				decimal[] damlist = battle.api_kouku.api_stage3.api_fdam;
				ChangeKoukuFlagToNumber(battle.api_kouku.api_stage3.api_frai_flag,
					battle.api_kouku.api_stage3.api_fbak_flag,
					numlist);
				DecimalListmake(numlist, damlist, lists);

				if (IsCombined)
				{
					if (battle.api_kouku.api_stage3_combined != null)
					{
						numlist = battle.api_kouku.api_stage3_combined.api_frai_flag;//뇌격

						damlist = battle.api_kouku.api_stage3_combined.api_fdam;
						ChangeKoukuFlagToNumber(battle.api_kouku.api_stage3_combined.api_frai_flag,
							battle.api_kouku.api_stage3_combined.api_fbak_flag,
							numlist);
						DecimalListmake(numlist, damlist, Combinelists);
					}//연합함대 리스트 작성 끝
				}
			}
			//항공전 끝

			//개막전 시작
			if (battle.api_opening_atack != null)
			{
				int[] numlist = battle.api_opening_atack.api_erai;
				decimal[] damlist = battle.api_opening_atack.api_eydam;
				DecimalListmake(numlist, damlist, lists);
			}
			//개막전 끝
			if (!IsCombined) BattleCalc(HPList, MHPList, lists, CurrentHPList, battle.api_maxhps, battle.api_nowhps);

			else if (IsCombined)//연합함대인경우 연산을 한번 더 시행
			{
				BattleCalc(HPList, MHPList, lists, CurrentHPList, battle.api_maxhps, battle.api_nowhps);
				//재활용 위해 초기화.
				CurrentHPList = new List<int>();
				HPList = new List<int>();
				MHPList = new List<int>();
				BattleCalc(HPList, MHPList, Combinelists, CurrentHPList, battle.api_maxhps_combined, battle.api_nowhps_combined);
			}
		}
		/// <summary>
		/// battle의 야전버전. Battle과 구조는 동일. 다만 전투의 양상이 조금 다르기때문에 분리. 연합함대와 아닌경우의 구분을 한다.
		/// </summary>
		/// <param name="battle"></param>
		/// <param name="IsCombined">연합함대인지 아닌지 입력합니다.연합함대인경우 True입니다.</param>
		private void MidBattle(bool IsCombined, kcsapi_midnight_battle battle)
		{
			if (!IsCombined) this.IsCritical = false;

			List<listup> lists = new List<listup>();
			List<int> HPList = new List<int>();
			List<int> MHPList = new List<int>();
			List<int> CurrentHPList = new List<int>();
			//포격전 리스트를 작성. 주간과 달리 1차 포격전밖에 없음.
			if (battle.api_hougeki != null)
				ObjectListmake(battle.api_hougeki.api_df_list, battle.api_hougeki.api_damage, lists);

			if (IsCombined && battle.api_maxhps_combined != null && battle.api_nowhps_combined != null)
				BattleCalc(HPList, MHPList, lists, CurrentHPList, battle.api_maxhps_combined, battle.api_nowhps_combined);
			else
				BattleCalc(HPList, MHPList, lists, CurrentHPList, battle.api_maxhps, battle.api_nowhps);
		}

		/// <summary>
		/// 전투결과 계산의 총계를 낸다.
		/// </summary>
		/// <param name="HPList">api_nowhps에서 필요한 값만 입력받을 빈 리스트</param>
		/// <param name="MHPList">api_maxhps에서 필요한 값만 입력받을 빈 리스트</param>
		/// <param name="lists">데미지를 모두 저장한 리스트를 입력.</param>
		/// <param name="CurrentHPList">계산이 끝난 HP리스트를 적제한다. HPList를 수정하는 방향이 더 좋겠지만...</param>
		/// <param name="Maxhps">api_maxhps를 가져온다.</param>
		/// <param name="NowHps">api_nowhps를 가져온다.</param>
		/// <param name="IsCombined">연합함대인지 설정</param>
		private void BattleCalc(List<int> HPList, List<int> MHPList, List<listup> lists, List<int> CurrentHPList, int[] Maxhps, int[] NowHps)
		{
			//총 HP와 현재 HP의 리스트를 작성한다. 빈칸과 적은 제외. 여기서 적까지 포함시키고 별도의 함수를 추가하면 전투 미리보기 구현도 가능
			for (int i = 0; i < 7; i++)
			{
				if (Maxhps[i] != -1) MHPList.Add(Maxhps[i]);
				if (NowHps[i] != -1) HPList.Add(NowHps[i]);
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
					this.IsCritical = true;
					break;
				}
			}
		}

		//리스트 작성부분은 좀 더 매끄러운 방법이 있는 것 같기도 하지만 일단 능력이 여기까지이므로
		/// <summary>
		/// decimal 데미지 리스트를 생성. 포격전이나 기타 컷인전투와 달리 Decimal값으로 바로 나온다.
		/// </summary>
		/// <param name="ho_target">타격대상 리스트를 입력합니다.</param>
		/// <param name="ho_damage">decimal[]형태의 데미지를 입력합니다.</param>
		/// <param name="list">출력할 데미지 리스트를 입력합니다.</param>
		private void DecimalListmake(int[] target, decimal[] damage, List<listup> list)
		{
			for (int i = 1; i < target.Count(); i++)//-1제외
			{
				listup d = new listup();
				d.Num = target[i];
				d.Damage = damage[i];
				if (d.Num != 0) list.Add(d);
			}

		}
		/// <summary>
		/// object 데미지 리스트를 생성. object로 포장되어 있어 이걸 분해하여 리스트로 넣어줍니다.
		/// </summary>
		/// <param name="target">타겟 리스트</param>
		/// <param name="damage">데미지 리스트. 한 리스트에 object로 0~2까지 포장되어있음. 보통은 1까지만 쓰이는 모양</param>
		/// <param name="list"></param>
		private void ObjectListmake(object[] target, object[] damage, List<listup> list)
		{
			for (int i = 1; i < target.Count(); i++)//-1제외
			{
				listup d = new listup();
				dynamic listNum = target[i];
				dynamic damNum = damage[i];
				d.Num = listNum[0];
				d.Damage = damNum[0];

				if (damNum.Length > 1)
				{
					if (damNum[1] > 0) d.Damage = damNum[0] + damNum[1];
				}
				list.Add(d);
			}

		}
		/// <summary>
		/// 항공전 Flag를 순번으로 바꿔줍니다. flag가 0이면 0으로 바꿉니다.
		/// </summary>
		/// <param name="rFlag">뇌격 flag입니다</param>
		/// <param name="bFlag">폭격 flag입니다</param>
		/// <param name="numlist">순번을 넣을 번호 리스트입니다.</param>
		private void ChangeKoukuFlagToNumber(int[] rFlag, int[] bFlag, int[] numlist)
		{
			for (int i = 0; i < rFlag.Count(); i++)//플래그를 번호로 변환
			{
				if (rFlag[i] == 1 || bFlag[i] == 1) numlist[i] = i;
				else numlist[i] = 0;
			}
		}
	}
}
