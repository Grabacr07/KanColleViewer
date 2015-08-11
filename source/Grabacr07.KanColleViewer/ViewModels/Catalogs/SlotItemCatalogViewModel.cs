using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemCatalogViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();

		public SlotItemCatalogWindowSettings Settings { get; }

		#region SlotItems 変更通知プロパティ

		private IReadOnlyCollection<SlotItemCounter> _SlotItems;

		public IReadOnlyCollection<SlotItemCounter> SlotItems
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
			this.Settings = new SlotItemCatalogWindowSettings();

			this.updateSource
				.Do(_ => this.IsReloading = true)
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Select(_ => UpdateCore())
				.Do(_ => this.IsReloading = false)
				.ObserveOnDispatcher()
				.Subscribe(x => this.SlotItems = x)
				.AddTo(this);

			this.Update();
		}

		public void Update()
		{
			this.updateSource.OnNext(Unit.Default);
		}

		private static List<SlotItemCounter> UpdateCore()
		{
			var ships = KanColleClient.Current.Homeport.Organization.Ships.Values.ToList();
			var items = KanColleClient.Current.Homeport.Itemyard.SlotItems.Values.ToList();
			var master = KanColleClient.Current.Master.SlotItems;

			// dic (Dictionary<TK,TV>)
			//  Key:   装備のマスター ID
			//  Value: Key が示す ID に該当する所有装備カウンター
			var dic = items
				.GroupBy(x => x.Info.Id)
				.ToDictionary(g => g.Key, g => new SlotItemCounter(master[g.Key], g));

			foreach (var ship in ships)
			{
				foreach (var target in ship.EquippedItems.Select(slot => new { slot, counter = dic[slot.Item.Info.Id] }))
				{
					target.counter.AddShip(ship, target.slot.Item.Level, target.slot.Item.Adept);
				}
			}

			return dic.Values
				.OrderBy(x => x.Target.CategoryId)
				.ThenBy(x => x.Target.Id)
				.ToList();
		}
	}
}
