using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;

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

		#region IsReloading 変更通知プロパティ

		private bool _IsReloading;

		public bool IsReloading
		{
			get { return this._IsReloading; }
			set
			{
				if (this._IsReloading != value)
				{
					this._IsReloading = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public SlotItemCatalogViewModel()
		{
			this.Title = "所有装備一覧";
			this.Update();
		}


		public async void Update()
		{
			this.IsReloading = true;
			this.SlotItems = await UpdateCore();
			this.IsReloading = false;
		}

		private static Task<List<SlotItemViewModel>> UpdateCore()
		{
			// これはひどい
			// あとでちゃんと書き直す

			var ships = KanColleClient.Current.Homeport.Organization.Ships;
			var items = KanColleClient.Current.Homeport.Itemyard.SlotItems;
			var master = KanColleClient.Current.Master.SlotItems;

			return Task.Factory.StartNew(() =>
			{
				var dic = items.GroupBy(kvp => kvp.Value.Info.Id, kvp => kvp.Value)
					.ToDictionary(g => g.Key, g => new SlotItemViewModel { SlotItem = master[g.Key], Count = g.Count() });

				foreach (var ship in ships.Values)
				{
					foreach (var target in ship.SlotItems.Where(x => x != null).Select(item => dic[item.Info.Id]))
					{
						target.AddShip(ship);
					}
				}

				return dic.Values
					.OrderBy(x => x.SlotItem.CategoryId)
					.ThenBy(x => x.SlotItem.Id)
					.ToList();
			});
		}
	}
}
