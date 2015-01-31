using System;
using System.Collections.Generic;
using System.IO;

namespace CsvLogReader
{
	public class LogDataList
	{
		public LogDataList()
		{

		}
		public List<DropStringLists> ReturnDropList(string FileName)
		{
			var items = new List<DropStringLists>();
			foreach (var line in File.ReadAllLines(FileName))
			{
				var parts = line.Split(',');
				if (parts[0] != "날짜")
				{
					items.Add(new DropStringLists
					{
						Date = parts[0],
						Drop = parts[1],
						SeaArea = parts[2],
						EnemyFleet = parts[3],
						Rank = parts[4],
					});
				}
			}
			return items;
		}
		public List<ItemStringLists> ReturnItemList(string FileName)
		{
			var items = new List<ItemStringLists>();
			foreach (var line in File.ReadAllLines(FileName))
			{
				var parts = line.Split(',');
				items.Add(new ItemStringLists
				{
					Date = parts[0],
					Results = parts[1],
					Assistant = parts[2],
					Fuel = parts[3],
					Bullet = parts[4],
					Steel = parts[5],
					bauxite = parts[6],
				});
			}
			return items;
		}
		public List<BuildStirngLists> ReturnBuildList(string FileName)
		{
			var items = new List<BuildStirngLists>();
			foreach (var line in File.ReadAllLines(FileName))
			{
				var parts = line.Split(',');
				items.Add(new BuildStirngLists
				{
					Date = parts[0],
					Results = parts[1],
					Fuel = parts[2],
					Bullet = parts[3],
					Steel = parts[4],
					bauxite = parts[5],
					UseItems = parts[6],
				});
			}
			return items;
		}
	}
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
	public class ItemStringLists{
		public string Date { get; set; }
		public string Results { get; set; }
		public string Assistant { get; set; }
		public string Fuel { get; set; }
		public string Steel { get; set; }
		public string Bullet { get; set; }
		public string bauxite { get; set; }
	}
	/// <summary>
	/// 날짜,결과,연료,탄,강재,보크사이트,개발자재
	/// </summary>
	public class BuildStirngLists
	{
		public string Date { get; set; }
		public string Results { get; set; }
		public string Fuel { get; set; }
		public string Steel { get; set; }
		public string Bullet { get; set; }
		public string bauxite { get; set; }
		public string UseItems { get; set; }
	}
}
