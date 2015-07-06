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

		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
			get { return KanColleClient.Current.Translations.GetTranslation(this.RawData.api_name, TranslationType.Equipment, false, this.RawData); }
		}

		public SlotItemType Type
		{
			get { return this.type ?? (SlotItemType)(this.type = (SlotItemType)(this.RawData.api_type.Get(2) ?? 0)); }
		}

		public SlotItemIconType IconType
		{
			get { return this.iconType ?? (SlotItemIconType)(this.iconType = (SlotItemIconType)(this.RawData.api_type.Get(3) ?? 0)); }
		}

		public int CategoryId
		{
			get { return this.categoryId ?? (int)(this.categoryId = this.RawData.api_type.Get(2) ?? int.MaxValue); }
		}

		/// <summary>
		/// 対空値を取得します。
		/// </summary>
		public int AA
		{
			get { return this.RawData.api_tyku; }
		}

		/// <summary>
		/// 制空戦に参加できる戦闘機または水上機かどうかを示す値を取得します。
		/// </summary>
		public bool IsAirSuperiorityFighter
		{
			get
			{
				return this.Type == SlotItemType.艦上戦闘機
					|| this.Type == SlotItemType.艦上攻撃機
					|| this.Type == SlotItemType.艦上爆撃機
					|| this.Type == SlotItemType.水上爆撃機;
			}
		}

		public bool IsNumerable
		{
			get
			{
				return this.Type == SlotItemType.艦上偵察機
					|| this.Type == SlotItemType.艦上戦闘機
					|| this.Type == SlotItemType.艦上攻撃機
					|| this.Type == SlotItemType.艦上爆撃機
					|| this.Type == SlotItemType.水上偵察機
					|| this.Type == SlotItemType.水上爆撃機;
			}
		}

		internal SlotItemInfo(kcsapi_mst_slotitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Type = {{{2}}}", this.Id, this.Name, this.RawData.api_type.ToString(", "));
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
		#region static members

		private static SlotItemInfo dummy = new SlotItemInfo(new kcsapi_mst_slotitem()
		{
			api_id = 0,
			api_name = "？？？",
		});

		public static SlotItemInfo Dummy
		{
			get { return dummy; }
		}

		#endregion
	}
}
