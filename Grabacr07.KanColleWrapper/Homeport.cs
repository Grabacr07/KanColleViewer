using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper.Models.Raw;
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
		/// 装備や消費アイテムの保有状況にアクセスできるようにします。
		/// </summary>
		public Itemyard Itemyard { get; private set; }

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



		internal Homeport(KanColleProxy proxy)
		{
			this.Materials = new Materials(proxy);
			this.Itemyard = new Itemyard(proxy);
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
			proxy.api_req_member_updatecomment.TryParse().Subscribe(this.UpdateComment);
		}


		internal void UpdateAdmiral(kcsapi_basic data)
		{
			this.Admiral = new Admiral(data);
		}

		private void UpdateComment(SvData data)
		{
			if (data == null || !data.IsSuccess) return;

			try
			{
				this.Admiral.Comment = data.Request["api_cmt"];
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("艦隊名の変更に失敗しました: {0}", ex);
			}
		}


		internal void StartConditionCount()
		{
			//Observable.Timer(TimeSpan.FromSeconds(10), TimeSpan.FromMinutes(3))
		}
		
	}
}
