
namespace Grabacr07.KanColleWrapper.Models
{   /// <summary>
	/// 날짜,해역이름,해역,보스,적 함대,랭크,드랍
	/// </summary>
	public class DropStringLists
	{
		public string Date { get; set; }
		public string SeaArea { get; set; }
		public string MapInfo { get; set; }
		public string Boss { get; set; }
		public string EnemyFleet { get; set; }
		public string Rank { get; set; }
		public string Drop { get; set; }
	}
	/// <summary>
	/// 날짜,비서함,연료,탄,강재,보크사이트,결과
	/// </summary>
	public class ItemStringLists
	{
		public string Date { get; set; }
		public string Assistant { get; set; }
		public int Fuel { get; set; }
		public int Steel { get; set; }
		public int Bullet { get; set; }
		public int bauxite { get; set; }
		public string Results { get; set; }
	}
	/// <summary>
	/// 날짜,결과,비서함,연료,탄,강재,보크사이트,개발자재
	/// </summary>
	public class BuildStirngLists
	{
		public string Date { get; set; }
		public string Assistant { get; set; }
		public int Fuel { get; set; }
		public int Steel { get; set; }
		public int Bullet { get; set; }
		public int bauxite { get; set; }
		public int UseItems { get; set; }
		public string Results { get; set; }
	}

}
