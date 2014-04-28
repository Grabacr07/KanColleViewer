using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 母港を表します。
	/// </summary>
	public class Homeport : NotificationObject
	{
		/// <summary>
		/// 艦隊の編成状況にアクセスできるようにします。
		/// </summary>
		public Organization Organization { get; private set; }

		/// <summary>
		/// 資源および資材の保有状況にアクセスできるようにします。
		/// </summary>
		public Materials Materials { get; private set; }

		/// <summary>
		/// 複数の建造ドックを持つ工廠を取得します。
		/// </summary>
		public Dockyard Dockyard { get; private set; }

		/// <summary>
		/// 複数の入渠ドックを持つ工廠を取得します。
		/// </summary>
		public Repairyard Repairyard { get; private set; }

		/// <summary>
		/// 任務情報を取得します。
		/// </summary>
		public Quests Quests { get; private set; }

		/// <summary>
		/// Logs events such as ship drops, crafts, and item developments.
		/// </summary>
		public Logger Logger { get; private set; }

		#region Admiral 変更通知プロパティ

		private Admiral _Admiral;

		/// <summary>
		/// 現在ログインしている提督を取得します。
		/// <see cref="INotifyPropertyChanged.PropertyChanged"/> イベントによる変更通知をサポートします。
		/// </summary>
		public Admiral Admiral
		{
			get { return this._Admiral; }
			private set
			{
				if (this._Admiral != value)
				{
					this._Admiral = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

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
					this.RaisePropertyChanged();
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


		internal Homeport(KanColleProxy proxy)
		{
			this.SlotItems = new MemberTable<SlotItem>();
			this.UseItems = new MemberTable<UseItem>();

			this.Materials = new Materials(proxy);
			this.Organization = new Organization(this, proxy);
			this.Repairyard = new Repairyard(this, proxy);
			this.Dockyard = new Dockyard(proxy);
			this.Quests = new Quests(proxy);
			this.Logger = new Logger(proxy);

			proxy.api_port.TryParse<kcsapi_port>().Subscribe(x =>
			{
				this.Organization.Update(x.Data.api_ship);
				this.Repairyard.Update(x.Data.api_ndock);
				this.Organization.Update(x.Data.api_deck_port);
				this.Materials.Update(x.Data.api_material);
			});
			proxy.api_get_member_basic.TryParse<kcsapi_basic>().Subscribe(x => this.UpdateAdmiral(x.Data));
			proxy.api_get_member_slot_item.TryParse<kcsapi_slotitem[]>().Subscribe(x => this.UpdateSlotItems(x.Data));
			proxy.api_get_member_useitem.TryParse<kcsapi_useitem[]>().Subscribe(x => this.UpdateUseItems(x.Data));
		}


		internal void UpdateAdmiral(kcsapi_basic data)
		{
			this.Admiral = new Admiral(data);
		}

		internal void UpdateSlotItems(kcsapi_slotitem[] source)
		{
			this.SlotItems = new MemberTable<SlotItem>(source.Select(x => new SlotItem(x)));
		}

		internal void UpdateUseItems(kcsapi_useitem[] source)
		{
			this.UseItems = new MemberTable<UseItem>(source.Select(x => new UseItem(x)));
		}
	}
}
