using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 装備アイテムの種類に基づく情報を表します。
	/// </summary>
	public class SlotItemInfo : RawDataWrapper<kcsapi_mst_slotitem>, IIdentifiable
	{
		private SlotItemType? type;
		private SlotItemIconType? iconType;
		private int? categoryId;

		public int Id => this.RawData.api_id;

		public string Name => this.RawData.api_name;

		public SlotItemType Type => this.type ?? (SlotItemType)(this.type = (SlotItemType)(this.RawData.api_type.Get(2) ?? 0));

		public SlotItemIconType IconType => this.iconType ?? (SlotItemIconType)(this.iconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0));

		public int CategoryId => this.categoryId ?? (int)(this.categoryId = this.RawData.api_type.Get(2) ?? int.MaxValue);

		/// <summary>
		/// 対空値を取得します。
		/// </summary>
		public int AA => this.RawData.api_tyku;

		/// <summary>
		/// 制空戦に参加できる戦闘機または水上機かどうかを示す値を取得します。
		/// </summary>
		public bool IsAirSuperiorityFighter => this.Type == SlotItemType.艦上戦闘機
											   || this.Type == SlotItemType.艦上攻撃機
											   || this.Type == SlotItemType.艦上爆撃機
											   || this.Type == SlotItemType.水上爆撃機;

		public bool IsNumerable => this.Type == SlotItemType.艦上偵察機
								   || this.Type == SlotItemType.艦上戦闘機
								   || this.Type == SlotItemType.艦上攻撃機
								   || this.Type == SlotItemType.艦上爆撃機
								   || this.Type == SlotItemType.水上偵察機
								   || this.Type == SlotItemType.水上爆撃機;

		internal SlotItemInfo(kcsapi_mst_slotitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", Type = {{{this.RawData.api_type.ToString(", ")}}}";
		}

		#region static members

	    public static SlotItemInfo Dummy { get; } = new SlotItemInfo(new kcsapi_mst_slotitem()
		{
		    api_id = 0,
		    api_name = "？？？",
		});

	    #endregion
	}
}
