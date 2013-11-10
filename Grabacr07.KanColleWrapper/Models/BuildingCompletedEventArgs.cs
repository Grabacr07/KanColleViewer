using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class BuildingCompletedEventArgs
	{
		/// <summary>
		/// 建造が完了したドックを一意に識別する ID を取得します。
		/// </summary>
		public int DockId { get; private set; }

		/// <summary>
		/// 建造された艦娘の種類を取得します。
		/// </summary>
		public ShipInfo Ship { get; private set; }

		public BuildingCompletedEventArgs(int id, ShipInfo ship)
		{
			this.DockId = id;
			this.Ship = ship;
		}
	}
}
