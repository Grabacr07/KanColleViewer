using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{	/// <summary>
	/// 날짜,드랍,해역,적 함대,랭크
	/// </summary>
	public class DropStringLists
	{
		public string Date { get; set; }
		public string Drop { get; set; }
		public string SeaArea { get; set; }
		public string EnemyFleet { get; set; }
		public string Rank { get; set; }
	}
	/// <summary>
	/// 날짜,결과,비서함,연료,탄,강재,보크사이트
	/// </summary>
	public class ItemStringLists
	{
		public string Date { get; set; }
		public string Results { get; set; }
		public string Assistant { get; set; }
		public int Fuel { get; set; }
		public int Steel { get; set; }
		public int Bullet { get; set; }
		public int bauxite { get; set; }
	}
	/// <summary>
	/// 날짜,결과,연료,탄,강재,보크사이트,개발자재
	/// </summary>
	public class BuildStirngLists
	{
		public string Date { get; set; }
		public string Results { get; set; }
		public int Fuel { get; set; }
		public int Steel { get; set; }
		public int Bullet { get; set; }
		public int bauxite { get; set; }
		public int UseItems { get; set; }
	}

}
