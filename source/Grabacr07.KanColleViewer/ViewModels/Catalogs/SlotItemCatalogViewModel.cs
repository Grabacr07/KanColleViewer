﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using MetroTrilithon.Mvvm;
using Grabacr07.KanColleWrapper.Models;
using Livet;

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

		public ItemLockFilter ItemLockFilter { get; }

		public SlotItemCatalogViewModel()
		{
			this.Title = "소유 장비 목록";
			this.Settings = new SlotItemCatalogWindowSettings();

			this.ItemLockFilter = new ItemLockFilter(this.Update);

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
			var ships = KanColleClient.Current.Homeport.Organization.Ships.Values;
			var items = KanColleClient.Current.Homeport.Itemyard.SlotItems.Values;
			var master = KanColleClient.Current.Master.SlotItems;

			items = items
				.Where(this.ItemLockFilter.Predicate);

			if (OnlyRemodeledSlotItems)
			{
				items = items
					.Where(x => x.Level > 0);
			}

			// dic (Dictionary<TK,TV>)
			//  Key:   装備のマスター ID
			//  Value: Key が示す ID に該当する所有装備カウンター
			var dic = items
				.GroupBy(x => x.Info.Id)
				.ToDictionary(g => g.Key, g => new SlotItemCounter(master[g.Key], g));

			foreach (var ship in ships)
			{
				var u = ship.EquippedItems
					.Where(this.ItemLockFilter.Predicate)
					.Where(y => items.Any(z => z.Id == y.Item.Id))
					.Select(slot => new { slot, counter = dic[slot.Item.Info.Id] });

				foreach (var target in u)
					target.counter.AddShip(ship, target.slot.Item.Level, target.slot.Item.Proficiency);
			}

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

	public abstract class ItemCatalogFilter : NotificationObject
	{
		private readonly Action action;

		public abstract bool Predicate(SlotItem item);
		public bool Predicate(ShipSlot item) => this.Predicate(item.Item);

		protected ItemCatalogFilter(Action updateAction)
		{
			this.action = updateAction;
		}

		protected void Update()
		{
			this.action?.Invoke();
		}
	}
	public class ItemLockFilter : ItemCatalogFilter
	{
		#region Both 変更通知プロパティ
		private bool _Both;
		public bool Both
		{
			get { return this._Both; }
			set
			{
				if (this._Both != value)
				{
					this._Both = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region Locked 変更通知プロパティ
		private bool _Locked;
		public bool Locked
		{
			get { return this._Locked; }
			set
			{
				if (this._Locked != value)
				{
					this._Locked = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		#region Unlocked 変更通知プロパティ
		private bool _Unlocked;
		public bool Unlocked
		{
			get { return this._Unlocked; }
			set
			{
				if (this._Unlocked != value)
				{
					this._Unlocked = value;
					this.RaisePropertyChanged();
					this.Update();
				}
			}
		}
		#endregion

		public ItemLockFilter(Action updateAction) : base(updateAction)
		{
			this._Both = true;
		}

		public override bool Predicate(SlotItem item)
		{
			if (this.Both) return true;
			if (this.Locked && item.Locked) return true;
			if (this.Unlocked && !item.Locked) return true;
			return false;
		}
	}
}
