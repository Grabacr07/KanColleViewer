using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 미리보기 데이터를 저장하는 클래스
	/// </summary>
	public class PreviewBattleResults
	{
		//이름
		public string EnemyName { get; set; }
		public string KanName { get; set; }
		public string SecondKanName { get; set; }
		//HP
		public string EnemyHP { get; set; }
		public string SecondKanHP { get; set; }
		public string KanHP { get; set; }
		//상태
		public string EnemyStatus { get; set; }
		public string SecondKanStatus { get; set; }
		public string KanStatus { get; set; }


		public string Rank { get; set; }
		//테스트용도
		//public int EnemyId { get; set; }
	}
	/// <summary>
	/// 예보 데이터를 저장하는 클래스입니다.
	/// </summary>
	public class ResultCalLists
	{
		public string RankString { get; set; }
		/// <summary>
		/// 심해서함의 ID를 저장. 심해서함의 이름을 출력하는데 필요
		/// </summary>
		public int[] Enemy { get; set; }
		/// <summary>
		/// 함대의 HP정보를 저장
		/// </summary>
		public List<string> HpResults = new List<string>();
		/// <summary>
		/// 연합함대의 HP정보를 저장
		/// </summary>
		public List<string> ComHpResults = new List<string>();
		/// <summary>
		/// 적 함대의 HP정보를 저장
		/// </summary>
		public List<string> EnemyHpResults = new List<string>();
		/// <summary>
		/// 함대의 HP계산결과를 저장
		/// </summary>
		public List<string> CalResults = new List<string>();
		/// <summary>
		/// 연합함대의 HP계산결과를 저장
		/// </summary>
		public List<string> ComCalResults = new List<string>();
		/// <summary>
		/// 적 함대의 HP계산결과를 저장
		/// </summary>
		public List<string> EnemyCalResults = new List<string>();
		/// <summary>
		/// 주간전의 아군피해를 저장
		/// </summary>
		public int KanDayBattleDamage { get; set; }
		/// <summary>
		/// 주간전의 적군피해를 저장
		/// </summary>
		public int EnemyDayBattleDamage { get; set; }
		
		//적
		/// <summary>
		/// 적군 굉침수 과반수 초과여부(2,4척인 경우는 0.5이상이면 true)
		/// </summary>
		public bool IsEnemyDeadOverHalf { get; set; }
		/// <summary>
		/// 적군 피해 유무
		/// </summary>
		public bool IsEnemyDamaged { get; set; }
		/// <summary>
		/// 적 전멸 여부
		/// </summary>
		public bool IsEnemyExterminated { get; set; }
		/// <summary>
		/// 적 기함 굉침
		/// </summary>
		public bool IsEnemyFlagDead { get; set; }


		//아군
		/// <summary>
		/// 아군 피해 유무
		/// </summary>
		public bool IsKanDamaged { get; set; }
		/// <summary>
		/// 아군굉침여부
		/// </summary>
		public bool IsKanDead { get; set; }


		//이외
		/// <summary>
		/// 전과게이지의 차이가 2.5배 이상
		/// </summary>
		public bool IsOverDamage { get; set; }
		/// <summary>
		/// 전과게이지 차이가 1배와 2.5배 사이
		/// </summary>
		public bool IsMidDamage { get; set; }
		/// <summary>
		/// 전과게이지 1미만
		/// </summary>
		public bool IsScratch { get; set; }
		/// <summary>
		/// 적군굉침>아군굉침
		/// </summary>
		public bool IsEnemyDeadOver { get; set; }
	}
}
