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
		public bool Succeed => this.RawData.api_create_flag == 1;

		public List<SlotItemInfo> SlotItemInfos { get; }

		public CreatedSlotItem(kcsapi_createitem rawData)
			: base(rawData)
		{
			try
			{
				this.SlotItemInfos = this.Succeed
					? new List<SlotItemInfo>(
						rawData.api_get_items
						.Where(x => x.api_slotitem_id != -1)
						.Select(x => KanColleClient.Current.Master.SlotItems[x.api_slotitem_id]))
					: null;

				//System.Diagnostics.Debug.WriteLine("createitem: {0} - {1}", this.Succeed, this.SlotItemInfos.Name);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
