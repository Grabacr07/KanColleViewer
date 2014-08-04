using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 工廠で開発された装備アイテムを表します。
	/// </summary>
	public class CreatedSlotItem : RawDataWrapper<kcsapi_createitem>
	{
		public bool Succeed
		{
			get { return this.RawData.api_create_flag == 1; }
		}

		public SlotItemInfo SlotItemInfo { get; private set; }

		public CreatedSlotItem(kcsapi_createitem rawData)
			: base(rawData)
		{
			try
			{
				this.SlotItemInfo = this.Succeed
					? KanColleClient.Current.Master.SlotItems[rawData.api_slot_item.api_slotitem_id]
					: KanColleClient.Current.Master.SlotItems[int.Parse(rawData.api_fdata.Split(',')[1])];

				System.Diagnostics.Debug.WriteLine("createitem: {0} - {1}", this.Succeed, this.SlotItemInfo.Name);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
