using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦娘の種類に基づく情報を表します。
	/// </summary>
	public class ShipInfo : RawDataWrapper<kcsapi_mst_ship>, IIdentifiable
	{
		private ShipType shipType;

		/// <summary>
		/// 艦を一意に識別する ID を取得します。
		/// </summary>
		public int Id => this.RawData.api_id;

		public int SortId => this.RawData.api_sortno;

		/// <summary>
		/// 艦の名称を取得します。
		/// </summary>
		public string Name => this.RawData.api_name;

		/// <summary>
		/// 艦種を取得します。
		/// </summary>
		public ShipType ShipType => this.shipType ?? (this.shipType = KanColleClient.Current.Master.ShipTypes[this.RawData.api_stype]) ?? ShipType.Dummy;

		/// <summary>
		/// 各装備スロットの最大搭載機数を取得します。
		/// </summary>
		public int[] Slots => this.RawData.api_maxeq;

		#region 用意したけど使わないっぽい？

		/// <summary>
		/// よみがなを取得します。
		/// </summary>
		public string Kana => this.RawData.api_yomi;

		/// <summary>
		/// 火力の最大値を取得します。
		/// </summary>
		public int MaxFirepower => this.RawData.api_houg.Get(1) ?? 0;

		/// <summary>
		/// 装甲の最大値を取得します。
		/// </summary>
		public int MaxArmer => this.RawData.api_souk.Get(1) ?? 0;

		/// <summary>
		/// 雷装の最大値を取得します。
		/// </summary>
		public int MaxTorpedo => this.RawData.api_raig.Get(1) ?? 0;

		/// <summary>
		/// 対空の最大値を取得します。
		/// </summary>
		public int MaxAA => this.RawData.api_tyku.Get(1) ?? 0;

		/// <summary>
		/// 運の最小値を取得します。
		/// </summary>
		public int MinLuck => this.RawData.api_luck.Get(0) ?? 0;

		/// <summary>
		/// 運の最大値を取得します。
		/// </summary>
		public int MaxLuck => this.RawData.api_luck.Get(1) ?? 0;

		/// <summary>
		/// 耐久値を取得します。
		/// </summary>
		public int HP => this.RawData.api_taik.Get(0) ?? 0;

		/// <summary>
		/// 燃料の最大値を取得します。
		/// </summary>
		public int MaxFuel => this.RawData.api_fuel_max;

		/// <summary>
		/// 弾薬の最大値を取得します。
		/// </summary>
		public int MaxBull => this.RawData.api_bull_max;

		/// <summary>
		/// 装備可能スロット数を取得します。
		/// </summary>
		public int SlotCount => this.RawData.api_slot_num;

		#endregion

		/// <summary>
		/// 速力を取得します。
		/// </summary>
		public ShipSpeed Speed => (ShipSpeed)this.RawData.api_soku;

		/// <summary>
		/// 次の改造が実施できるレベルを取得します。
		/// </summary>
		public int? NextRemodelingLevel => this.RawData.api_afterlv == 0 ? null : (int?)this.RawData.api_afterlv;


		internal ShipInfo(kcsapi_mst_ship rawData) : base(rawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", ShipType = \"{this.ShipType.Name}\"";
		}

		#region static members

		public static ShipInfo Dummy { get; } = new ShipInfo(new kcsapi_mst_ship
		{
			api_id = 0,
			api_name = "？？？"
		});

		#endregion
	}
}
