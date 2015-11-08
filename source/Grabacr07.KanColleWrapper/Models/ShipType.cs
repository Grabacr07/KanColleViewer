using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦種を表します。
	/// </summary>
	public class ShipType : RawDataWrapper<kcsapi_mst_stype>, IIdentifiable
	{
		public int Id => this.RawData.api_id;

		public string Name => this.RawData.api_name;

		public int SortNumber => this.RawData.api_sortno;

		public ShipType(kcsapi_mst_stype rawData) : base(rawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\"";
		}

		public double RepairMultiplier
		{
			get
			{
				switch ((ShipTypeId)this.Id)
				{
					case ShipTypeId.Submarine:
						return 0.5;
					case ShipTypeId.HeavyCruiser:
					case ShipTypeId.AerialCruiser:
					case ShipTypeId.FastBattleship:
					case ShipTypeId.LightAircraftCarrier:
					case ShipTypeId.SubmarineTender:
						return 1.5;
					case ShipTypeId.Battleship:
					case ShipTypeId.Superdreadnought:
					case ShipTypeId.AerialBattleship:
					case ShipTypeId.AircraftCarrier:
					case ShipTypeId.ArmoredAircraftCarrier:
					case ShipTypeId.RepairShip:
						return 2;
					default:
						return 1;
				}
			}
		}

		#region static members

		public static ShipType Dummy { get; } = new ShipType(new kcsapi_mst_stype
		{
		    api_id = 999,
		    api_sortno = 999,
		    api_name = "不審船",
		});

		#endregion

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

			/// <summary>
			/// 揚陸艦
			/// </summary>
			AmphibiousAssault = 17,

			/// <summary>
			/// 装甲空母
			/// </summary>
			ArmoredAircraftCarrier = 18,

			/// <summary>
			/// 工作艦
			/// </summary>
			RepairShip = 19,

			/// <summary>
			/// 潜水母艦
			/// </summary>
			SubmarineTender = 20,

			TrainingCruiser = 21,

			FleetOiler = 22,
		}
	}
}
