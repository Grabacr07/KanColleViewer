using System.Collections.Generic;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 미리보기 데이터를 저장하는 클래스
	/// </summary>
	public class PreviewBattleResults
	{
		//이름
		public string Name { get; set; }
		//LimitedValue
		public LimitedValue HP { get; set; }
		//상태
		public int Status { get; set; }
		//레벨
		public int Lv { get; set; }
		public int CHP { get; set; }
		public int MHP { get; set; }
	}
	/// <summary>
	/// 예보 데이터를 저장하는 클래스입니다.
	/// </summary>
	public class ResultCalLists
	{
		public int DockId { get; set; }
		public int RankInt { get; set; }
		/// <summary>
		/// 적의 ID를 저장. 적의 이름을 출력하는데 필요
		/// </summary>
		public int[] EnemyID { get; set; }
		/// <summary>
		/// 적의 레벨을 저장.
		/// </summary>
		public int[] EnemyLv { get; set; }
		/// <summary>
		/// 함대의 HP정보를 저장
		/// </summary>
		public List<int> HpResults = new List<int>();
		/// <summary>
		/// 연합함대의 HP정보를 저장
		/// </summary>
		public List<int> ComHpResults = new List<int>();
		/// <summary>
		/// 적 함대의 HP정보를 저장
		/// </summary>
		public List<int> EnemyHpResults = new List<int>();		
		/// <summary>
		/// 함대의 HP정보를 저장
		/// </summary>
		public List<int> MHpResults = new List<int>();
		/// <summary>
		/// 연합함대의 HP정보를 저장
		/// </summary>
		public List<int> ComMHpResults = new List<int>();
		/// <summary>
		/// 적 함대의 HP정보를 저장
		/// </summary>
		public List<int> EnemyMHpResults = new List<int>();
		/// <summary>
		/// 함대의 HP계산결과를 저장
		/// </summary>
		public List<int> CalResults = new List<int>();
		/// <summary>
		/// 연합함대의 HP계산결과를 저장
		/// </summary>
		public List<int> ComCalResults = new List<int>();
		/// <summary>
		/// 적 함대의 HP계산결과를 저장
		/// </summary>
		public List<int> EnemyCalResults = new List<int>();
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
		public int FirstKanDamaged { get; set; }
		public int FirstKanMaxHP { get; set; }
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
