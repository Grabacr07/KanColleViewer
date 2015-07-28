using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 装備タイプを表します。
	/// </summary>
	public class SlotItemEquipType : RawDataWrapper<kcsapi_mst_slotitem_equiptype>, IIdentifiable
	{
		public int Id { get; }

		public string Name { get; }

		public SlotItemEquipType(kcsapi_mst_slotitem_equiptype rawData)
			: base(rawData)
		{
			this.Id = rawData.api_id;
			this.Name = rawData.api_name;
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Name}\"";
		}

		#region static members

		public static SlotItemEquipType Dummy { get; } = new SlotItemEquipType(new kcsapi_mst_slotitem_equiptype
		{
			api_id = 0,
			api_name = "？？？",
		});

		#endregion
	}
}
