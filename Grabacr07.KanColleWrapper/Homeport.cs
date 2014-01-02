using System;
using System.Collections.Generic;
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
		#region Fleets 変更通知プロパティ

		private MemberTable<Fleet> _Fleets;

		public MemberTable<Fleet> Fleets
		{
			get { return this._Fleets; }
			set
			{
				if (this._Fleets != value)
				{
					this._Fleets = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

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

		#region Admiral 変更通知プロパティ

		private Admiral _Admiral;

		/// <summary>
		/// 現在ログインしている提督を取得します。
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

		#region Materials 変更通知プロパティ

		private Materials _Materials;

		/// <summary>
		/// 艦隊司令部の資源および資材の保有状況にアクセスできるようにします。
		/// </summary>
		public Materials Materials
		{
			get { return this._Materials; }
			set
			{
				if (this._Materials != value)
				{
					this._Materials = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Ships 変更通知プロパティ

		private MemberTable<Ship> _Ships;

		/// <summary>
		/// 艦隊司令部に所属しているすべての艦娘を取得します。艦娘の ID を使用して添え字アクセスできます。
		/// </summary>
		public MemberTable<Ship> Ships
		{
			get { return this._Ships; }
			set
			{
				if (this._Ships != value)
				{
					this._Ships = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region SlotItems 変更通知プロパティ

		private MemberTable<SlotItem> _SlotItems;

		/// <summary>
		/// 艦隊司令部が保有しているすべての装備を取得します。装備の ID を使用して添え字アクセスできます。
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
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/basic")
				.TryParse<kcsapi_basic>()
				.Subscribe(x => this.Admiral = new Admiral(x));

			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/material")
				.TryParse<kcsapi_material[]>()
				.Subscribe(x => this.Materials = new Materials(x.Select(m => new Material(m)).ToArray()));

			this.Ships = new MemberTable<Ship>();
			this.Fleets = new MemberTable<Fleet>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/ship")
				.Select(x => { SvData<kcsapi_ship2[]> result; return SvData.TryParse(x, out result) ? result : null; })
				.Where(x => x != null && x.IsSuccess)
				.Subscribe(x => this.Ships = new MemberTable<Ship>(x.Data.Select(s => new Ship(this, s))));

			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/ship2")
				.Select(x => { SvData<kcsapi_ship2[]> result; return SvData.TryParse(x, out result) ? result : null; })
				.Where(x => x != null && x.IsSuccess)
				.Subscribe(x =>
				{
					this.Ships = new MemberTable<Ship>(x.Data.Select(s => new Ship(this, s)));
					this.UpdateFleets(x.Fleets);
				});
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/ship3")
				.TryParse<kcsapi_ship3>()
				.Subscribe(x =>
				{
					this.Ships = new MemberTable<Ship>(x.api_ship_data.Select(s => new Ship(this, s)));
					this.UpdateFleets(x.api_deck_data);
				});

			this.SlotItems = new MemberTable<SlotItem>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/slotitem")
				.TryParse<kcsapi_slotitem[]>()
				.Subscribe(x => this.SlotItems = new MemberTable<SlotItem>(x.Select(s => new SlotItem(s))));

			this.UseItems = new MemberTable<UseItem>();
			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/useitem")
				.TryParse<kcsapi_useitem[]>()
				.Subscribe(x => this.UseItems = new MemberTable<UseItem>(x.Select(s => new UseItem(s))));

			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/deck")
				.TryParse<kcsapi_deck[]>()
				.Subscribe(this.UpdateFleets);

			proxy.ApiSessionSource.Where(x => x.PathAndQuery == "/kcsapi/api_get_member/deck_port")
				.TryParse<kcsapi_deck[]>()
				.Subscribe(this.UpdateFleets);

			this.Dockyard = new Dockyard(proxy);
			this.Repairyard = new Repairyard(this, proxy);
			this.Quests = new Quests(proxy);
		}


		private void UpdateFleets(kcsapi_deck[] source)
		{
			if (this.Fleets.Count == source.Length)
			{
				foreach (var raw in source)
				{
					var target = this.Fleets[raw.api_id];
					if (target != null) target.Update(raw);
				}
			}
			else
			{
				this.Fleets.ForEach(x => x.Value.Dispose());
				this.Fleets = new MemberTable<Fleet>(source.Select(x => new Fleet(this, x)));
			}
		}
	}
}
