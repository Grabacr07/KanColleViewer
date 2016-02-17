using System;
using System.Collections.Generic;
using System.Linq;
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

		#region ReturnTime / Remaining / IsInExecution 変更通知プロパティ

		private DateTimeOffset? _ReturnTime;
		/// <summary>
		/// 遠征から帰還する日時を取得します。
		/// </summary>
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
					this.RaisePropertyChanged(nameof(this.Remaining));
					this.RaisePropertyChanged(nameof(this.IsInExecution));
				}
			}
		}

		/// <summary>
		/// 遠征から帰還するまでの時間を取得します。
		/// </summary>
		public TimeSpan? Remaining
			=> !this.ReturnTime.HasValue ? (TimeSpan?)null
			: this.ReturnTime.Value < DateTimeOffset.Now ? TimeSpan.Zero
			: this.ReturnTime.Value - DateTimeOffset.Now;

		/// <summary>
		/// 現在遠征を実行中かどうかを示す値を取得します。
		/// </summary>
		public bool IsInExecution => this.ReturnTime.HasValue;

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
			this.RaisePropertyChanged(nameof(this.Remaining));

			if (!this.notificated
				&& this.Returned != null
				&& this.Remaining <= TimeSpan.FromSeconds(KanColleClient.Current.Settings.NotificationShorteningTime))
			{
				this.Returned(this, new ExpeditionReturnedEventArgs(this.fleet.Name));
				this.notificated = true;
			}
		}

		protected override void Tick()
		{
			base.Tick();
			this.UpdateCore();
		}
	}
}
