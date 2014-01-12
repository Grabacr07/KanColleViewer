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
	public class ShipInfo : RawDataWrapper<kcsapi_master_ship>, IIdentifiable
	{
		private ShipType shipType;

		/// <summary>
		/// 艦を一意に識別する ID を取得します。
		/// </summary>
		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public int SortId
		{
			get { return this.RawData.api_sortno; }
		}

		/// <summary>
		/// 艦の名称を取得します。
		/// </summary>
		public string Name
		{
			get { return this.RawData.api_name; }
		}

		/// <summary>
		/// 艦種を取得します。
		/// </summary>
		public ShipType ShipType
		{
			get { return shipType ?? (shipType = KanColleClient.Current.Master.ShipTypes[this.RawData.api_stype]) ?? ShipType.Dummy; }
		}

		#region 用意したけど使わないっぽい？

		/// <summary>
		/// 火力の最大値を取得します。
		/// </summary>
		public int MaxFirepower
		{
			get { return this.RawData.api_houg.Get(1) ?? 0; }
		}

		/// <summary>
		/// 装甲の最大値を取得します。
		/// </summary>
		public int MaxArmer
		{
			get { return this.RawData.api_souk.Get(1) ?? 0; }
		}

		/// <summary>
		/// 雷装の最大値を取得します。
		/// </summary>
		public int MaxTorpedo
		{
			get { return this.RawData.api_raig.Get(1) ?? 0; }
		}

		/// <summary>
		/// 対空の最大値を取得します。
		/// </summary>
		public int MaxAA
		{
			get { return this.RawData.api_tyku.Get(1) ?? 0; }
		}


		/// <summary>
		/// 耐久値を取得します。
		/// </summary>
		public int HP
		{
			get { return this.RawData.api_taik.Get(0) ?? 0; }
		}

		/// <summary>
		/// 回避の最大値を取得します。
		/// </summary>
		public int MaxEvasion
		{
			get { return this.RawData.api_kaih.Get(1) ?? 0; }
		}

		/// <summary>
		/// 対潜の最大値を取得します (ASW: Anti-submarine warfare)。
		/// </summary>
		public int MaxASW
		{
			get { return this.RawData.api_tais.Get(1) ?? 0; }
		}

		/// <summary>
		/// 索敵の最大値を取得します。
		/// </summary>
		public int MaxLOS
		{
			get { return this.RawData.api_saku.Get(1) ?? 0; }
		}

		#endregion

		/// <summary>
		/// 速力を取得します。
		/// </summary>
		public Speed Speed
		{
			get { return (Speed)this.RawData.api_sokuh; }
		}

		internal ShipInfo(kcsapi_master_ship rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", ShipType = \"{2}\"", this.Id, this.Name, this.ShipType.Name);
		}

		#region static members

		private static readonly ShipInfo dummy = new ShipInfo(new kcsapi_master_ship
		{
			api_id = 0,
			api_name = "？？？"
		});

		public static ShipInfo Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
