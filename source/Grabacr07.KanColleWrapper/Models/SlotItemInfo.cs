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

		public string Name => KanColleClient.Current.Translations.GetTranslation(this.RawData.api_name, TranslationType.Equipment, false, this.RawData);

		public SlotItemType Type => this.type ?? (SlotItemType)(this.type = (SlotItemType)(this.RawData.api_type.Get(2) ?? 0));

		public SlotItemIconType IconType => this.iconType ?? (SlotItemIconType)(this.iconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0));

		public int CategoryId => this.categoryId ?? (int)(this.categoryId = this.RawData.api_type.Get(2) ?? int.MaxValue);

		/// <summary>
		/// 火力値を取得します。
		/// </summary>
		public int Firepower => this.RawData.api_houg;

		/// <summary>
		/// 装甲値を取得します。
		/// </summary>
		public int Armer => this.RawData.api_souk;

		/// <summary>
		/// 雷装値を取得します。
		/// </summary>
		public int Torpedo => this.RawData.api_raig;

		/// <summary>
		/// 対空値を取得します。
		/// </summary>
		public int AA => this.RawData.api_tyku;

		/// <summary>
		/// 爆装値を取得します。
		/// </summary>
		public int Bomb => this.RawData.api_baku;

		/// <summary>
		/// 対潜値を取得します。
		/// </summary>
		public int ASW => this.RawData.api_tais;

		/// <summary>
		/// 命中値を取得します。
		/// </summary>
		public int Hit => this.RawData.api_houm;

		/// <summary>
		/// 回避値を取得します。
		/// </summary>
		public int Evade => this.RawData.api_houk;

		/// <summary>
		/// 索敵値を取得します。
		/// </summary>
		public int ViewRange => this.RawData.api_saku;

		/// <summary>
		/// 制空戦に参加できる戦闘機または水上機かどうかを示す値を取得します。
		/// </summary>
		public bool IsAirSuperiorityFighter => this.Type == SlotItemType.艦上戦闘機
											   || this.Type == SlotItemType.艦上攻撃機
											   || this.Type == SlotItemType.艦上爆撃機
											   || this.Type == SlotItemType.水上爆撃機
											   || this.Type == SlotItemType.水上戦闘機;

		public bool IsNumerable => this.Type == SlotItemType.艦上偵察機
								   || this.Type == SlotItemType.艦上戦闘機
								   || this.Type == SlotItemType.艦上攻撃機
								   || this.Type == SlotItemType.艦上爆撃機
								   || this.Type == SlotItemType.水上偵察機
								   || this.Type == SlotItemType.水上爆撃機
								   || this.Type == SlotItemType.水上戦闘機;

		public bool IsFirstEncounter => this.Type == SlotItemType.艦上偵察機
									   || this.Type == SlotItemType.水上偵察機;

		public bool IsSecondEncounter => this.Type == SlotItemType.艦上偵察機
								   || this.Type == SlotItemType.艦上攻撃機
								   || this.Type == SlotItemType.水上偵察機;

		public double SecondEncounter => this.ViewRange * 0.07;

		public SlotItemEquipType EquipType { get; }

		internal SlotItemInfo(kcsapi_mst_slotitem rawData, MasterTable<SlotItemEquipType> types) : base(rawData)
		{
			this.EquipType = types[rawData.api_type?[2] ?? 0] ?? SlotItemEquipType.Dummy;
		}
		public string ToolTipData
		{
			get
			{
				var tooltip = (this.RawData.api_houg != 0 ? "화력:" + this.RawData.api_houg : "")
					   + (this.RawData.api_raig != 0 ? " 뇌장:" + this.RawData.api_raig : "")
					   + (this.AA != 0 ? " 대공:" + this.AA : "")
					   + (this.RawData.api_souk != 0 ? " 장갑:" + this.RawData.api_souk : "")
					   + (this.RawData.api_baku != 0 ? " 폭장:" + this.RawData.api_baku : "")
					   + (this.RawData.api_tais != 0 ? " 대잠:" + this.RawData.api_tais : "")
					   + (this.RawData.api_houm != 0 ? " 명중:" + this.RawData.api_houm : "")
					   + (this.RawData.api_houk != 0 ? " 회피:" + this.RawData.api_houk : "")
					   + (this.RawData.api_saku != 0 ? " 색적:" + this.RawData.api_saku : "");
				if (tooltip.Length < 1) tooltip = "스테이터스 없음";
				return tooltip;
			}
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", Type = {{{this.RawData.api_type.ToString(", ")}}}";
		}

		#region static members

	    public static SlotItemInfo Dummy { get; } = new SlotItemInfo(new kcsapi_mst_slotitem()
		{
		    api_id = 0,
		    api_name = "？？？",
		}, new MasterTable<SlotItemEquipType>());

	    #endregion
	}
}
