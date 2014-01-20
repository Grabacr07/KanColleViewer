using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// プレイヤー データに依存しないマスター情報を表します。
	/// </summary>
	public class Master
	{
		/// <summary>
		/// すべての艦娘の定義を取得します。
		/// </summary>
		public MasterTable<ShipInfo> Ships { get; private set; }

		/// <summary>
		/// すべての装備アイテムの定義を取得します。
		/// </summary>
		public MasterTable<SlotItemInfo> SlotItems { get; private set; }

		/// <summary>
		/// すべての消費アイテムの定義を取得します。
		/// </summary>
		public MasterTable<UseItemInfo> UseItems { get; private set; }

		/// <summary>
		/// 艦種を取得します。
		/// </summary>
		public MasterTable<ShipType> ShipTypes { get; private set; }


		internal Master(KanColleProxy proxy)
		{
			this.Ships = new MasterTable<ShipInfo>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_master/ship")
				.TryParse<kcsapi_master_ship[]>()
				.Subscribe(x => this.Ships = new MasterTable<ShipInfo>(x.Select(s => new ShipInfo(s))));

			this.SlotItems = new MasterTable<SlotItemInfo>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_master/slotitem")
				.TryParse<kcsapi_master_slotitem[]>()
				.Subscribe(x => this.SlotItems = new MasterTable<SlotItemInfo>(x.Select(s => new SlotItemInfo(s))));

			this.UseItems = new MasterTable<UseItemInfo>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_master/useitem")
				.TryParse<kcsapi_master_useitem[]>()
				.Subscribe(x => this.UseItems = new MasterTable<UseItemInfo>(x.Select(s => new UseItemInfo(s))));

			this.ShipTypes = new MasterTable<ShipType>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_master/stype")
				.TryParse<kcsapi_stype[]>()
				.Subscribe(x => this.ShipTypes = new MasterTable<ShipType>(x.Select(s => new ShipType(s))));
		}
	}
}
