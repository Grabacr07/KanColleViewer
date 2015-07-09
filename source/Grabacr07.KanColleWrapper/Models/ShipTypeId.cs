using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
	public enum ShipTypeId
	{
		Unknown = 0,

		/// <summary>
		/// 海防艦。
		/// </summary>
		EscortShip = 1,

		/// <summary>
		/// 駆逐艦。
		/// </summary>
		Destroyer = 2,

		/// <summary>
		/// 軽巡洋艦。
		/// </summary>
		LightCruiser = 3,

		/// <summary>
		/// 重雷装巡洋艦。
		/// </summary>
		TorpedoCruiser = 4,

		/// <summary>
		/// 重巡洋艦。
		/// </summary>
		HeavyCruiser = 5,

		/// <summary>
		/// 航空巡洋艦。
		/// </summary>
		AerialCruiser = 6,

		/// <summary>
		/// 軽空母。
		/// </summary>
		LightAircraftCarrier = 7,

		/// <summary>
		/// 高速戦艦。
		/// </summary>
		FastBattleship = 8,

		/// <summary>
		/// 戦艦。
		/// </summary>
		Battleship = 9,
		
		/// <summary>
		/// 航空戦艦。
		/// </summary>
		AerialBattleship = 10,

		/// <summary>
		/// 正規空母。
		/// </summary>
		AircraftCarrier = 11,

		/// <summary>
		/// 超弩級戦艦。
		/// </summary>
		Superdreadnought = 12,

		/// <summary>
		/// 潜水艦。
		/// </summary>
		Submarine = 13,

		/// <summary>
		/// 潜水空母。
		/// </summary>
		AircraftCarryingSubmarine = 14,

		/// <summary>
		/// 補給艦。
		/// </summary>
		ReplenishmentOiler = 15,

		/// <summary>
		/// 水上機母艦。
		/// </summary>
		SeaplaneCarrier = 16,
	}
}
