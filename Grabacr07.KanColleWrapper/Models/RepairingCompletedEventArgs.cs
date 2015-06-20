using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public class RepairingCompletedEventArgs
	{
		/// <summary>
		/// 入渠が完了したドックを一意に識別する ID を取得します。
		/// </summary>
		public int DockId { get; private set; }

		/// <summary>
		/// 入渠が完了した艦娘を取得します。
		/// </summary>
		public Ship Ship { get; private set; }

		public RepairingCompletedEventArgs(int id, Ship ship)
		{
			this.DockId = id;
			this.Ship = ship;
		}
	}
}
