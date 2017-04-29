using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class SlotItemCatalogViewModel : WindowViewModel
	{
		private readonly Subject<Unit> updateSource = new Subject<Unit>();

		public SlotItemCatalogWindowSettings Settings { get; }

		public IReadOnlyCollection<SlotItemEquipTypeViewModel> SlotItemEquipTypes { get; }
		public IEnumerable<SlotItemIconType> EnableSlotItemEquipTypes => SlotItemEquipTypes
			.Where(x => x.IsSelected)
			.Select(y => y.Type);

		public bool CheckAllSlotItemEquipType
		{
			get { return this.SlotItemEquipTypes.All(x => x.IsSelected); }
			set
			{
				foreach (var type in this.SlotItemEquipTypes) type.Set(value);
				this.Update();
			}
		}

		private bool _OnlyRemodeledSlotItems;
		public bool OnlyRemodeledSlotItems
		{
			get { return _OnlyRemodeledSlotItems; }
			set
			{
				if(_OnlyRemodeledSlotItems != value)
				{
					_OnlyRemodeledSlotItems = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}

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


		#region IsOpenFilterSettings 変更通知プロパティ

		private bool _IsOpenFilterSettings;

		public bool IsOpenFilterSettings
		{
			get { return this._IsOpenFilterSettings; }
			set
			{
				if (this._IsOpenFilterSettings != value)
				{
					this._IsOpenFilterSettings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public SlotItemCatalogViewModel()
		{
			this.Title = "소유 장비 목록";
			this.Settings = new SlotItemCatalogWindowSettings();

			/*
			this.SlotItemEquipTypes = KanColleClient.Current.Master.SlotItemEquipTypes
				.Select(kvp => new SlotItemEquipTypeViewModel(kvp.Value)
				{
					IsSelected = true,
					SelectionChangedAction = () => this.Update()
				})
				.ToList();
			*/
			this.SlotItemEquipTypes = Enum.GetNames(typeof(SlotItemIconType))
				.Select(x => new SlotItemEquipTypeViewModel((SlotItemIconType)Enum.Parse(typeof(SlotItemIconType), x))
				{
					IsSelected = true,
					SelectionChangedAction = () => this.Update()
				})
				.Distinct(x => x.DisplayName)
				.ToList();

			this.updateSource
				.Do(_ => this.IsReloading = true)
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Select(_ => UpdateCore(EnableSlotItemEquipTypes))
				.Do(_ => this.IsReloading = false)
				.ObserveOnDispatcher()
				.Subscribe(x => this.SlotItems = x)
				.AddTo(this);

			CheckAllSlotItemEquipType = true;
		}

		public void Update()
		{
			this.RaisePropertyChanged(nameof(this.CheckAllSlotItemEquipType));
			this.updateSource.OnNext(Unit.Default);
		}

		private List<SlotItemCounter> UpdateCore(IEnumerable<SlotItemIconType> enableSlotItemEquipTypes)
		{
			var ships = KanColleClient.Current.Homeport.Organization.Ships.Values.ToList();
			var items = KanColleClient.Current.Homeport.Itemyard.SlotItems.Values.ToList();
			var master = KanColleClient.Current.Master.SlotItems;

			if (OnlyRemodeledSlotItems)
			{
				items = items.Where(x => x.Level > 0)
					.ToList();

				ships = ships.Select(
					x => {
						x.EquippedItems = x.EquippedItems
							.Where(y => y.Item.Level > 0 && items.Any(z => z.Info.Id == y.Item.Info.Id))
							.ToArray();
						return x;
					})
					.ToList();
			}

			// dic (Dictionary<TK,TV>)
			//  Key:   装備のマスター ID
			//  Value: Key が示す ID に該当する所有装備カウンター
			var dic = items
				.GroupBy(x => x.Info.Id)
				.ToDictionary(g => g.Key, g => new SlotItemCounter(master[g.Key], g));

			foreach (var ship in ships)
				foreach (var target in ship.EquippedItems.Select(slot => new { slot, counter = dic[slot.Item.Info.Id] }))
					target.counter.AddShip(ship, target.slot.Item.Level, target.slot.Item.Proficiency);

			return dic.Values
				.Where(w => enableSlotItemEquipTypes.Contains(GetIconTypeInRange(w.Target.IconType)))
				.OrderBy(x => x.Target.CategoryId)
				.ThenBy(x => x.Target.Id)
				.ToList();
		}
		private static SlotItemIconType GetIconTypeInRange(SlotItemIconType source)
		{
			var z = SlotItemEquipTypeViewModel.IconAliasNamable.ContainsKey(source)
						? SlotItemEquipTypeViewModel.IconAliasNamable[source]
						: source;

			var y = (Enum.GetValues(typeof(SlotItemIconType)) as int[])
				.Any(x => x == (int) (z));

			if (!y) return SlotItemIconType.Unknown;
			return z;
		}

		public void SetSlotItemEquipType(int[] ids)
		{
			foreach (var type in this.SlotItemEquipTypes)
				type.Set(ids.Any(y => y == (int)type.Type));
			this.Update();
		}
	}
}
