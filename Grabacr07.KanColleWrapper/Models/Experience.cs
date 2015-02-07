﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 経験値テーブルを表します。
	/// </summary>
	public class Experience
	{
		public int Level { get; private set; }
		public int Next { get; private set; }
		public int Total { get; }

		private Experience(int level, int next, int total)
		{
			this.Level = level;
			this.Next = next;
			this.Total = total;
		}

		#region table

		private static readonly Dictionary<int, Experience> table = new Dictionary<int, Experience>
		{
			{ 1, new Experience(1, 100, 0) },
			{ 2, new Experience(2, 200, 100) },
			{ 3, new Experience(3, 300, 300) },
			{ 4, new Experience(4, 400, 600) },
			{ 5, new Experience(5, 500, 1000) },
			{ 6, new Experience(6, 600, 1500) },
			{ 7, new Experience(7, 700, 2100) },
			{ 8, new Experience(8, 800, 2800) },
			{ 9, new Experience(9, 900, 3600) },
			{ 10, new Experience(10, 1000, 4500) },
			{ 11, new Experience(11, 1100, 5500) },
			{ 12, new Experience(12, 1200, 6600) },
			{ 13, new Experience(13, 1300, 7800) },
			{ 14, new Experience(14, 1400, 9100) },
			{ 15, new Experience(15, 1500, 10500) },
			{ 16, new Experience(16, 1600, 12000) },
			{ 17, new Experience(17, 1700, 13600) },
			{ 18, new Experience(18, 1800, 15300) },
			{ 19, new Experience(19, 1900, 17100) },
			{ 20, new Experience(20, 2000, 19000) },
			{ 21, new Experience(21, 2100, 21000) },
			{ 22, new Experience(22, 2200, 23100) },
			{ 23, new Experience(23, 2300, 25300) },
			{ 24, new Experience(24, 2400, 27600) },
			{ 25, new Experience(25, 2500, 30000) },
			{ 26, new Experience(26, 2600, 32500) },
			{ 27, new Experience(27, 2700, 35100) },
			{ 28, new Experience(28, 2800, 37800) },
			{ 29, new Experience(29, 2900, 40600) },
			{ 30, new Experience(30, 3000, 43500) },
			{ 31, new Experience(31, 3100, 46500) },
			{ 32, new Experience(32, 3200, 49600) },
			{ 33, new Experience(33, 3300, 52800) },
			{ 34, new Experience(34, 3400, 56100) },
			{ 35, new Experience(35, 3500, 59500) },
			{ 36, new Experience(36, 3600, 63000) },
			{ 37, new Experience(37, 3700, 66600) },
			{ 38, new Experience(38, 3800, 70300) },
			{ 39, new Experience(39, 3900, 74100) },
			{ 40, new Experience(40, 4000, 78000) },
			{ 41, new Experience(41, 4100, 82000) },
			{ 42, new Experience(42, 4200, 86100) },
			{ 43, new Experience(43, 4300, 90300) },
			{ 44, new Experience(44, 4400, 94600) },
			{ 45, new Experience(45, 4500, 99000) },
			{ 46, new Experience(46, 4600, 103500) },
			{ 47, new Experience(47, 4700, 108100) },
			{ 48, new Experience(48, 4800, 112800) },
			{ 49, new Experience(49, 4900, 117600) },
			{ 50, new Experience(50, 5000, 122500) },
			{ 51, new Experience(51, 5200, 127500) },
			{ 52, new Experience(52, 5400, 132700) },
			{ 53, new Experience(53, 5600, 138100) },
			{ 54, new Experience(54, 5800, 143700) },
			{ 55, new Experience(55, 6000, 149500) },
			{ 56, new Experience(56, 6200, 155500) },
			{ 57, new Experience(57, 6400, 161700) },
			{ 58, new Experience(58, 6600, 168100) },
			{ 59, new Experience(59, 6800, 174700) },
			{ 60, new Experience(60, 7000, 181500) },
			{ 61, new Experience(61, 7300, 188500) },
			{ 62, new Experience(62, 7600, 195800) },
			{ 63, new Experience(63, 7900, 203400) },
			{ 64, new Experience(64, 8200, 211300) },
			{ 65, new Experience(65, 8500, 219500) },
			{ 66, new Experience(66, 8800, 228000) },
			{ 67, new Experience(67, 9100, 236800) },
			{ 68, new Experience(68, 9400, 245900) },
			{ 69, new Experience(69, 9700, 255300) },
			{ 70, new Experience(70, 10000, 265000) },
			{ 71, new Experience(71, 10400, 275000) },
			{ 72, new Experience(72, 10800, 285400) },
			{ 73, new Experience(73, 11200, 296200) },
			{ 74, new Experience(74, 11600, 307400) },
			{ 75, new Experience(75, 12000, 319000) },
			{ 76, new Experience(76, 12400, 331000) },
			{ 77, new Experience(77, 12800, 343400) },
			{ 78, new Experience(78, 13200, 356200) },
			{ 79, new Experience(79, 13600, 369400) },
			{ 80, new Experience(80, 14000, 383000) },
			{ 81, new Experience(81, 14500, 397000) },
			{ 82, new Experience(82, 15000, 411500) },
			{ 83, new Experience(83, 15500, 426500) },
			{ 84, new Experience(84, 16000, 442000) },
			{ 85, new Experience(85, 16500, 458000) },
			{ 86, new Experience(86, 17000, 474500) },
			{ 87, new Experience(87, 17500, 491500) },
			{ 88, new Experience(88, 18000, 509000) },
			{ 89, new Experience(89, 18500, 527000) },
			{ 90, new Experience(90, 19000, 545500) },
			{ 91, new Experience(91, 20000, 564500) },
			{ 92, new Experience(92, 22000, 584500) },
			{ 93, new Experience(93, 25000, 606500) },
			{ 94, new Experience(94, 30000, 631500) },
			{ 95, new Experience(95, 40000, 661500) },
			{ 96, new Experience(96, 60000, 701500) },
			{ 97, new Experience(97, 90000, 761500) },
			{ 98, new Experience(98, 148500, 851500) },
			{ 99, new Experience(99, 300000, 1000000) },
			{ 100, new Experience(100, 300000, 1300000) },
			{ 101, new Experience(101, 300000, 1600000) },
			{ 102, new Experience(102, 300000, 1900000) },
			{ 103, new Experience(103, 400000, 2200000) },
			{ 104, new Experience(104, 400000, 2600000) },
			{ 105, new Experience(105, 500000, 3000000) },
			{ 106, new Experience(106, 500000, 3500000) },
			{ 107, new Experience(107, 600000, 4000000) },
			{ 108, new Experience(108, 600000, 4600000) },
			{ 109, new Experience(109, 700000, 5200000) },
			{ 110, new Experience(110, 700000, 5900000) },
			{ 111, new Experience(111, 800000, 6600000) },
			{ 112, new Experience(112, 800000, 7400000) },
			{ 113, new Experience(113, 900000, 8200000) },
			{ 114, new Experience(114, 900000, 9100000) },
			{ 115, new Experience(115, 1000000, 10000000) },
			{ 116, new Experience(116, 1000000, 11000000) },
			{ 117, new Experience(117, 1000000, 12000000) },
			{ 118, new Experience(118, 1000000, 13000000) },
			{ 119, new Experience(119, 1000000, 14000000) },
			{ 120, new Experience(120, 0, 15000000) }
		};

		#endregion

		/// <summary>
		/// 指定した艦隊司令部のレベルにおいて、次のレベルに上がるために必要な経験値を取得します。
		/// </summary>
		/// <param name="currentLevel">艦隊司令部の現在のレベル。</param>
		/// <param name="currentExperience">艦隊司令部の現在のレベル。</param>
		/// <returns><paramref name="currentLevel" /> から次のレベルに上がるために必要となる経験値。</returns>
		public static int GetAdmiralExpForNextLevel(int currentLevel, int currentExperience)
		{
			return table.ContainsKey(currentLevel + 1) ? table[currentLevel + 1].Total - currentExperience : 0;
		}
	}
}
