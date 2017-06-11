using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
		public string TypeName => KanColleClient.Current.Translations.GetTranslation(this.Type.ToString(), TranslationType.EquipmentTypes, false, this.RawData);

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
		/// 基地航空隊の航続距離値を取得します。
		/// </summary>
		public int Distance => this.RawData.api_distance;

		public bool IsNumerable => this.Type.IsNumerable();

		public bool IsAerialCombatable // 항공전 참여 여부
		{
			get
			{
				switch (this.Type)
				{
					case SlotItemType.艦上戦闘機: // 전투기
					case SlotItemType.水上戦闘機: // 수상전투기
					case SlotItemType.噴式戦闘機: // 분식전투기
					case SlotItemType.局地戦闘機: // 국지전투기
					case SlotItemType.艦上攻撃機: // 뇌격기
					case SlotItemType.艦上爆撃機: // 폭격기
					case SlotItemType.噴式攻撃機: // 분식뇌격기
					case SlotItemType.噴式戦闘爆撃機: // 분식전투폭격기
					case SlotItemType.陸上攻撃機: // 육상공격기
					case SlotItemType.水上爆撃機: // 수상폭격기
						return true;
				}
				return false;
			}
		}

		public bool IsFirstEncounter => this.Type == SlotItemType.艦上偵察機
									   || this.Type == SlotItemType.水上偵察機;

		public bool IsSecondEncounter => this.Type == SlotItemType.艦上偵察機
								   || this.Type == SlotItemType.艦上攻撃機
								   || this.Type == SlotItemType.水上偵察機;

		public double SecondEncounter => this.ViewRange * 0.07;

		public int AirBaseCost
		{
			get
			{
				var cost = this.RawData.api_cost;
				if (cost == -1) return cost;

				var type = this.Type;
				if (type == SlotItemType.None) return cost;

				switch (type)
				{
					case SlotItemType.艦上偵察機:
					case SlotItemType.水上偵察機:
					case SlotItemType.噴式偵察機:
					case SlotItemType.大型飛行艇:
						return cost * 4;

					default:
						return cost * 12;
				}
			}
		}

		public SlotItemEquipType EquipType { get; }

		internal SlotItemInfo(kcsapi_mst_slotitem rawData, MasterTable<SlotItemEquipType> types) : base(rawData)
		{
			this.EquipType = types[rawData.api_type?[2] ?? 0] ?? SlotItemEquipType.Dummy;
		}
		public string ToolTipData
		{
			get
			{
				var tooltip = string.Join(
					Environment.NewLine,
					new string[] {
						(this.RawData.api_houg != 0 ? "화력: " + this.RawData.api_houg : ""),
						(this.RawData.api_raig != 0 ? "뇌장: " + this.RawData.api_raig : ""),
						(this.RawData.api_tyku != 0 ? "대공: " + this.RawData.api_tyku : ""),
						(this.RawData.api_souk != 0 ? "장갑: " + this.RawData.api_souk : ""),
						(this.RawData.api_baku != 0 ? "폭장: " + this.RawData.api_baku : ""),
						(this.RawData.api_tais != 0 ? "대잠: " + this.RawData.api_tais : ""),
						(
							this.Type == SlotItemType.局地戦闘機
							? (this.RawData.api_houm != 0 ? "대폭: " + this.RawData.api_houm : "")
							: (this.RawData.api_houm != 0 ? "명중: " + this.RawData.api_houm : "")
						),
						(
							this.Type == SlotItemType.局地戦闘機
							? (this.RawData.api_houk != 0 ? "영격: " + this.RawData.api_houk : "")
							: (this.RawData.api_houk != 0 ? "회피: " + this.RawData.api_houk : "")
						),
						(this.RawData.api_saku != 0 ? "색적: " + this.RawData.api_saku : ""),
						(this.IsNumerable ? " " : ""),
						(this.IsNumerable ? "항속거리: " + this.RawData.api_distance : ""),
						(this.IsNumerable ? "배치비용: " + this.AirBaseCost : "")
					}.Where(x => x.Length > 0)
				);
				if (tooltip.Length < 1) tooltip = "스테이터스 없음";

				return tooltip;
			}
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\", Type = {{{this.RawData.api_type.ToString(", ")}}}";
		}

		#region static members

		public static SlotItemInfo Dummy { get; } = new SlotItemInfo(
			new kcsapi_mst_slotitem()
			{
				api_id = 0,
				api_name = "？？？",
			},
			new MasterTable<SlotItemEquipType>());

		#endregion
	}
}
