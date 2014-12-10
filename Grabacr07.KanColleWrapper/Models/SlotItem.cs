using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class SlotItem : RawDataWrapper<kcsapi_slotitem>, IIdentifiable
	{
		public int Id
		{
			get { return this.RawData.api_id; }
		}

		public SlotItemInfo Info { get; private set; }
		public int Level { get { return this.RawData.api_level; } }
		public string ItemLv { get { return "+"+Level.ToString(); } }

		internal SlotItem(kcsapi_slotitem rawData) : base(rawData)
		{
			this.Info = KanColleClient.Current.Master.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Dummy;
		}

		public override string ToString()
		{
			return string.Format("ID = {0}, Name = \"{1}\"", this.Id, this.Info.Name);
		}
	}
}
