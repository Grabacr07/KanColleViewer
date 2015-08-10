using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper.Models
{
	public class SlotItem : RawDataWrapper<kcsapi_slotitem>, IIdentifiable
	{
		public int Id => this.RawData.api_id;

		public SlotItemInfo Info { get; private set; }

		public int Level => this.RawData.api_level;

		public string LevelText => this.Level >= 10 ? "★max" : this.Level >= 1 ? ("★+" + this.Level) : "";

		public string NameWithLevel => $"{this.Info.Name}{(this.Level >= 1 ? (" " + this.LevelText) : "")}{(this.Adept >= 1 ? (" " + this.AdeptText) : "")}";

		public int Adept => this.RawData.api_alv;

		public string AdeptText => this.Adept >= 1 ? (" (熟練度 " + this.Adept + ")") : "";

		internal SlotItem(kcsapi_slotitem rawData)
			: base(rawData)
		{
			this.Info = KanColleClient.Current.Master.SlotItems[this.RawData.api_slotitem_id] ?? SlotItemInfo.Dummy;
		}


		public void Remodel(int level, int masterId)
		{
			this.RawData.api_level = level;
			this.Info = KanColleClient.Current.Master.SlotItems[masterId] ?? SlotItemInfo.Dummy;

			this.RaisePropertyChanged(nameof(this.Info));
			this.RaisePropertyChanged(nameof(this.Level));
		}

		public override string ToString()
		{
			return $"ID = {this.Id}, Name = \"{this.Info.Name}\", Level = {this.Level}, Adapt = {this.Adept}";
		}


		public static SlotItem Dummy { get; } = new SlotItem(new kcsapi_slotitem { api_slotitem_id = -1, });
	}
}
