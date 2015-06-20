﻿using System;
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
	public class RepairingDock : TimerNotifier, IIdentifiable
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

		#region ShipId 変更通知プロパティ

		private int _ShipId;

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
				}
			}
		}

		#endregion

		#region Ship 変更通知プロパティ

		private Ship target;

		/// <summary>
		/// 入渠中の艦娘の情報を取得します。
		/// </summary>
		public Ship Ship
		{
			get { return this.target; }
			private set
			{
				if (this.target != value)
				{
					var oldShip = this.target;
					var newShip = value;
					if (oldShip != null) oldShip.Situation &= ~ShipSituation.Repair;
					if (newShip != null) newShip.Situation |= ShipSituation.Repair;

					this.target = value;
					this.RaisePropertyChanged();
				}
			}
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
			this.Ship = this.State == RepairingDockState.Repairing ? this.homeport.Organization.Ships[this.ShipId] : null;
			this.CompleteTime = this.State == RepairingDockState.Repairing
				? (DateTimeOffset?)Definitions.UnixEpoch.AddMilliseconds(rawData.api_complete_time)
				: null;
		}

		internal void Finish()
		{
			this.State = RepairingDockState.Unlocked;
			this.ShipId = -1;
			this.Ship = null;
			this.CompleteTime = null;
		}


		protected override void Tick()
		{
			base.Tick();

			if (this.CompleteTime.HasValue)
			{
				var remaining = this.CompleteTime.Value - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated
					&& this.Completed != null
					&& remaining <= TimeSpan.FromSeconds(KanColleClient.Current.Settings.NotificationShorteningTime))
				{
					this.Completed(this, new RepairingCompletedEventArgs(this.Id, this.Ship));
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
