using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 遠征を表します。
	/// </summary>
	public class Expedition : TimerNotifier, IIdentifiable
	{
		private readonly Fleet fleet;
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

		#region Mission 変更通知プロパティ

		private Mission _Mission;

		/// <summary>
		/// 実行中の遠征任務を取得します。
		/// </summary>
		public Mission Mission
		{
			get { return this._Mission; }
			private set
			{
				if (this._Mission != value)
				{
					this._Mission = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ReturnTime / IsInExecution 変更通知プロパティ

		private DateTimeOffset? _ReturnTime;

		public DateTimeOffset? ReturnTime
		{
			get { return this._ReturnTime; }
			private set
			{
				if (this._ReturnTime != value)
				{
					this._ReturnTime = value;
					this.notificated = false;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("IsInExecution");
				}
			}
		}


		/// <summary>
		/// 現在遠征を実行中かどうかを示す値を取得します。
		/// </summary>
		public bool IsInExecution => this.ReturnTime.HasValue;

	    #endregion

		#region Remaining 変更通知プロパティ

		private TimeSpan? _Remaining;

		public TimeSpan? Remaining
		{
			get { return this._Remaining; }
			set
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
		/// 艦隊が遠征から帰ったときに発生します。
		/// </summary>
		public event EventHandler<ExpeditionReturnedEventArgs> Returned;


		public Expedition(Fleet fleet)
		{
			this.fleet = fleet;
		}

		internal void Update(long[] rawData)
		{
			if (rawData.Length != 4 || rawData.All(x => x == 0))
			{
				this.Id = -1;
				this.Mission = null;
				this.ReturnTime = null;
				this.Remaining = null;
			}
			else
			{
				this.Id = (int)rawData[1];
				this.Mission = KanColleClient.Current.Master.Missions[this.Id];
				this.ReturnTime = Definitions.UnixEpoch.AddMilliseconds(rawData[2]);
				this.UpdateCore();
			}
		}

		private void UpdateCore()
		{
			if (this.ReturnTime.HasValue)
			{
				var remaining = this.ReturnTime.Value - DateTimeOffset.Now;
				if (remaining.Ticks < 0) remaining = TimeSpan.Zero;

				this.Remaining = remaining;

				if (!this.notificated
					&& this.Returned != null
					&& remaining <= TimeSpan.FromSeconds(KanColleClient.Current.Settings.NotificationShorteningTime))
				{
					this.Returned(this, new ExpeditionReturnedEventArgs(this.fleet.Name));
					this.notificated = true;
				}
			}
			else
			{
				this.Remaining = null;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			this.UpdateCore();
		}
	}
}
