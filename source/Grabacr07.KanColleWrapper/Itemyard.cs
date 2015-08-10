using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;

namespace Grabacr07.KanColleWrapper
{
	public class Itemyard : Notifier
	{

		/// <summary>
		/// <see cref="SlotItems"/> の装備数を取得します。
		/// </summary>
		public int SlotItemsCount => this.SlotItems.Count;

		#region SlotItems 変更通知プロパティ

		private MemberTable<SlotItem> _SlotItems;

		/// <summary>
		/// 艦隊司令部が保有しているすべての装備を取得します。
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントによる変更通知をサポートします。
		/// </summary>
		public MemberTable<SlotItem> SlotItems
		{
			get { return this._SlotItems; }
			set
			{
				if (this._SlotItems != value)
				{
					this._SlotItems = value;
					this.RaiseSlotItemsChanged();
				}
			}
		}

		#endregion

		#region UseItems 変更通知プロパティ

		private MemberTable<UseItem> _UseItems;

		/// <summary>
		/// 母港が所有するすべての消費アイテムを取得します。
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントによる変更通知をサポートします。
		/// </summary>
		public MemberTable<UseItem> UseItems
		{
			get { return this._UseItems; }
			set
			{
				if (this._UseItems != value)
				{
					this._UseItems = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal Itemyard(KanColleProxy proxy)
		{
			this.SlotItems = new MemberTable<SlotItem>();
			this.UseItems = new MemberTable<UseItem>();

			proxy.api_get_member_slot_item.TryParse<kcsapi_slotitem[]>().Subscribe(x => this.Update(x.Data));
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(x => this.CreateItem(x.Data));
			proxy.api_req_kousyou_destroyitem2.TryParse<kcsapi_destroyitem2>().Subscribe(this.DestroyItem);
			// 出撃中の装備数調整は諦め！

			proxy.api_get_member_useitem.TryParse<kcsapi_useitem[]>().Subscribe(x => this.Update(x.Data));

			proxy.api_req_kousyou_remodel_slot.TryParse<kcsapi_remodel_slot>().Subscribe(x =>
			{
				this.RemoveFromRemodel(x.Data);
				this.RemodelSlotItem(x.Data);
			});
		}


		internal void Update(kcsapi_slotitem[] source)
		{
			this.SlotItems = new MemberTable<SlotItem>(source.Select(x => new SlotItem(x)));
		}

		internal void Update(kcsapi_useitem[] source)
		{
			this.UseItems = new MemberTable<UseItem>(source.Select(x => new UseItem(x)));
		}

		internal void AddFromDock(kcsapi_kdock_getship source)
		{
			foreach (var x in source.api_slotitem.Select(x => new SlotItem(x)))
			{
				this.SlotItems.Add(x);
			}
			this.RaiseSlotItemsChanged();
		}

		internal void RemoveFromShip(Ship ship)
		{
			foreach (var x in ship.EquippedItems.ToArray())
			{
				this.SlotItems.Remove(x.Item);
			}
			this.RaiseSlotItemsChanged();
		}

		internal void RemoveFromRemodel(kcsapi_remodel_slot source)
		{
			if (source.api_use_slot_id != null)
			{
				foreach (var id in source.api_use_slot_id)
				{
					this.SlotItems.Remove(id);
				}
				this.RaiseSlotItemsChanged();
			}
		}

		private void CreateItem(kcsapi_createitem source)
		{
			if (source.api_create_flag == 1 && source.api_slot_item != null)
			{
				this.SlotItems.Add(new SlotItem(source.api_slot_item));
			}
			this.RaiseSlotItemsChanged();
		}

		private void DestroyItem(SvData<kcsapi_destroyitem2> data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				foreach (var x in data.Request["api_slotitem_ids"].Split(',').Select(int.Parse))
				{
					this.SlotItems.Remove(x);
				}
				this.RaiseSlotItemsChanged();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("装備の破棄に失敗しました: {0}", ex);
			}
		}

		private void RemodelSlotItem(kcsapi_remodel_slot source)
		{
			if (source.api_after_slot == null) return;

			this.SlotItems[source.api_after_slot.api_id]
				?.Remodel(source.api_after_slot.api_level, source.api_after_slot.api_slotitem_id);
		}


		private void RaiseSlotItemsChanged()
		{
			this.RaisePropertyChanged(nameof(this.SlotItems));
			this.RaisePropertyChanged(nameof(this.SlotItemsCount));
		}
	}
}
