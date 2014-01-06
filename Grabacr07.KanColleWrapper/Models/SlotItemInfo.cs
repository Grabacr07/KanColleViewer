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
	public class SlotItemInfo : RawDataWrapper<kcsapi_master_slotitem>, IIdentifiable
	{
		private SlotItemIconType? iconType;

		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public string Name
		{
			get { return this.RawData.api_name; }
		}

		public SlotItemIconType IconType
		{
			get { return this.iconType ?? (this.iconType = this.RawData.api_type.Length == 4 ? (SlotItemIconType)this.RawData.api_type[3] : SlotItemIconType.Unknown).Value; }
		}

		/// <summary>
		/// 対空値を取得します。
		/// </summary>
		public int AA
		{
			get { return this.RawData.api_tyku; }
		}

		/// <summary>
		/// この装備アイテムが艦載機かどうかを示す値を取得します。
		/// </summary>
		public bool IsAircraft
		{
			get
			{
				return this.IconType == SlotItemIconType.Fighter ||
					   this.IconType == SlotItemIconType.TorpedoBomber ||
					   this.IconType == SlotItemIconType.DiveBomber ||
					   this.IconType == SlotItemIconType.ReconPlane;
			}
		}

		/// <summary>
		/// この装備アイテムが水上機かどうかを示す値を取得します。
		/// </summary>
		public bool IsSeaplane
		{
			get { return this.IconType == SlotItemIconType.ReconSeaplane; }
		}

		internal SlotItemInfo(kcsapi_master_slotitem rawData) : base(rawData) { }

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\", Type = {{{2}}}", this.Id, this.Name, this.RawData.api_type.ToString(", "));
		}

		#region static members

		private static readonly SlotItemInfo dummy = new SlotItemInfo(new kcsapi_master_slotitem()
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
