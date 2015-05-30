using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 複数の建造ドックを持つ工廠を表します。
	/// </summary>
	public class Dockyard : NotificationObject
	{
		#region Dock 変更通知プロパティ

		private MemberTable<BuildingDock> _Docks;

		public MemberTable<BuildingDock> Docks
		{
			get { return this._Docks; }
			set
			{
				if (this._Docks != value)
				{
					this._Docks = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CreatedSlotItem 変更通知プロパティ

		private CreatedSlotItem _CreatedSlotItem;
		
		/// <summary>
		/// 最後に開発された装備アイテムの情報を取得します。
		/// </summary>
		public CreatedSlotItem CreatedSlotItem
		{
			get { return this._CreatedSlotItem; }
			private set
			{
				if (this._CreatedSlotItem != value)
				{
					this._CreatedSlotItem = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		internal Dockyard(KanColleProxy proxy)
		{
			this.Docks = new MemberTable<BuildingDock>();

			proxy.api_get_member_kdock.TryParse<kcsapi_kdock[]>().Subscribe(x => this.Update(x.Data));
			proxy.api_req_kousyou_getship.TryParse<kcsapi_kdock_getship>().Subscribe(x => this.GetShip(x.Data));
			proxy.api_req_kousyou_createship_speedchange.TryParse().Subscribe(this.ChangeSpeed);
			proxy.api_req_kousyou_createitem.TryParse<kcsapi_createitem>().Subscribe(this.CreateSlotItem);
		}


		internal void Update(kcsapi_kdock[] source)
		{
			if (this.Docks.Count == source.Length)
			{
				foreach (var raw in source)
				{
					var target = this.Docks[raw.api_id];
					if (target != null) target.Update(raw);
				}
			}
			else
			{
				this.Docks.ForEach(x => x.Value.Dispose());
				this.Docks = new MemberTable<BuildingDock>(source.Select(x => new BuildingDock(x)));
			}
		}

		private void GetShip(kcsapi_kdock_getship source)
		{
			this.Update(source.api_kdock);
		}

		private void ChangeSpeed(SvData svd)
		{
			try
			{
				var dock = this.Docks[int.Parse(svd.Request["api_kdock_id"])];
				var highspeed = svd.Request["api_highspeed"] == "1";

				if (highspeed) dock.Finish();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("高速建造材使用の解析に失敗しました: {0}", ex);
			}
		}

		private void CreateSlotItem(SvData<kcsapi_createitem> svd)
		{
			this.CreatedSlotItem = new CreatedSlotItem(svd.Data);
		}
	}
}
