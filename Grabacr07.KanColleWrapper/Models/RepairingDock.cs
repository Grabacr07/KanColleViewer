using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models.Raw;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 入渠ドックを表します。
	/// </summary>
	public class RepairingDock : TimerNotificator, IIdentifiable
	{
		private readonly Homeport homeport;
		private bool notificated;

		#region Id 変更通知プロパティ

		private int _Id;

		public int Id
		{
			get { return this._Id; }
			private set
			{
				if (this._Id != value)
				{
					this._Id = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region State 変更通知プロパティ

		private RepairingDockState _State;

		public RepairingDockState State
		{
			get { return this._State; }
			private set
			{
				if (this._State != value)
				{
					this._State = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipId / Ship 変更通知プロパティ

		private int _ShipId;
		private Ship _Ship;

		/// <summary>
		/// 入渠中の艦娘を一意に識別する ID を取得します。
		/// </summary>
		public int ShipId
		{
			get { return this._ShipId; }
			private set
			{
				if (this._ShipId != value)
				{
					this._ShipId = value;
					this.RaisePropertyChanged();
					this.UpdateShip();
				}
			}
		}

		/// <summary>
		/// 入渠中の艦娘の情報を取得します。
		/// </summary>
		public Ship Ship
		{
			get { return this.State == RepairingDockState.Repairing ? this._Ship : null; }
		}

		#endregion

		#region CompleteTime 変更通知プロパティ

		private DateTimeOffset? _CompleteTime;

		/// <summary>
		/// 入渠完了時刻を取得します。
		/// </summary>
		public DateTimeOffset? CompleteTime
		{
			get { return this._CompleteTime; }
			private set
			{
				if (this._CompleteTime != value)
				{
					this._CompleteTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Remaining 変更通知プロパティ

		private TimeSpan? _Remaining;

		/// <summary>
		/// 入渠が完了するまでの残り時間を取得します。1 秒ごとに更新されます。
		/// </summary>
		public TimeSpan? Remaining
		{
			get { return this._Remaining; }
			private set
			{
				if (this._Remaining != value)
				{
					this._Remaining = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		/// <summary>
		/// 入渠が完了したときに発生します。
		/// </summary>
		public event EventHandler<RepairingCompletedEventArgs> Completed;

		internal RepairingDock(Homeport parent, kcsapi_ndock rawData)
		{
			this.homeport = parent;
			this.Update(rawData);
		}

		internal void Update(kcsapi_ndock rawData)
		{
			this.Id = rawData.api_id;
			this.State = (RepairingDockState)rawData.api_state;
			this.ShipId = rawData.api_ship_id;
			this.CompleteTime = this.State == RepairingDockState.Repairing
				? (DateTimeOffset?)Definitions.UnixEpoch.AddMilliseconds(rawData.api_complete_time)
				: null;

			// ShipId の Setter で呼び出されるため、ここで UpdateShip() は呼び出さない
			// 何らかの原因で Ship の作成に失敗した場合は Tick() 時に再設定する
		}

		// Setterで処理できないため、更新用関数を作成。
		internal void UpdateShip()
		{
			this._Ship = this.homeport.Ships[this.ShipId];
			this.RaisePropertyChanged(() => this.Ship);
		}

		protected override void Tick()
		{
			base.Tick();

			if (this.CompleteTime.HasValue)
			{
				var remaining = this.CompleteTime.Value - TimeSpan.FromMinutes(1.0) - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				// 何らかの原因で Ship の作成に失敗していた場合に再設定
				// 初回起動時等 this.homeport.Ships が不定のタイミングで発生
				if (null == this.Ship)
				{
					this.UpdateShip();
				}

				if (!this.notificated && this.Completed != null && remaining.Ticks <= 0)
				{
					this.Completed(this, new RepairingCompletedEventArgs(this.Id));
					this.notificated = true;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}
	}
}
