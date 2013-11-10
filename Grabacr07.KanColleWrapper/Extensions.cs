using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	public static class Extensions
	{
		/// <summary>
		/// 現在の艦隊に所属している艦娘を取得します。
		/// </summary>
		/// <param name="fleet"></param>
		/// <returns></returns>
		public static IEnumerable<Ship> GetShips(this Fleet fleet)
		{
			return fleet.Ships.Where(x => x != null);
		}  
	}
}
