using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemCatalogViewModel : WindowViewModel
	{
		#region SlotItems 変更通知プロパティ

		private IReadOnlyCollection<SlotItemViewModel> _SlotItems;

		public IReadOnlyCollection<SlotItemViewModel> SlotItems
		{
			get { return this._SlotItems; }
			set
			{
				if (this._SlotItems != value)
				{
					this._SlotItems = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public SlotItemCatalogViewModel()
		{
			this.Title = "所有装備一覧";
			this.UpdateCore();
		}


		private void UpdateCore()
		{
			// これはひどい
			// あとでちゃんと書き直す

			var ships = KanColleClient.Current.Homeport.Ships;
			var items = KanColleClient.Current.Homeport.SlotItems;
			var dic = KanColleClient.Current.Master.SlotItems.ToDictionary(kvp => kvp.Key, kvp => new SlotItemViewModel(kvp.Value));

			items.ForEach(x => dic[x.Value.Info.Id].Count++);

			foreach (var ship in ships.Values)
			{
				foreach (var target in ship.SlotItems.Where(x => x != null).Select(item => dic[item.Info.Id]))
				{
					target.Ships.Add(ship);
				}
			}

			this.SlotItems = dic.Values.Where(x => x.Count > 0).ToList();
		}
	}


	public class SlotItemViewModel : ViewModel
	{
		public SlotItemInfo SlotItem { get; set; }
		public int Count { get; set; }
		public List<Ship> Ships { get; set; }

		public SlotItemViewModel(SlotItemInfo info)
		{
			this.SlotItem = info;
			this.Ships = new List<Ship>();
		}

		public override string ToString()
		{
			return string.Format(
				"{0} - Count: {1} / Ships: {2}", 
				this.SlotItem.Name, this.Count, 
				string.Join(",", this.Ships.Select(x => x.Info.Name + " (Lv." + x.Level + ")")));
		}
	}
}
