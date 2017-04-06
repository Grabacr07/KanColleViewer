using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models
{
	public class AkashiTimer : TimerNotifier
	{
		#region BaseTime 변경 통지 프로퍼티
		private DateTimeOffset _BaseTime;
		public DateTimeOffset BaseTime
		{
			get { return _BaseTime; }
			private set
			{
				if(_BaseTime != value)
				{
					_BaseTime = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		private double Elapsed => DateTimeOffset.Now.Subtract(this.BaseTime).TotalMinutes;

		private bool Notified { get; set; }
		public event EventHandler Repaired;
		public event EventHandler TimerTick;

		public AkashiTimer()
		{
			this.BaseTime = DateTimeOffset.Now;
			this.Notified = false;
		}

		public void Update(int fleetId, int shipIdx, int shipId)
		{
			if (shipIdx == -1) return;

			var akashiCheck = false;
			var fleets = KanColleClient.Current.Homeport.Organization.Fleets;
			var firstShip = fleets[fleetId]?.Ships[0]?.Info.Id ?? 0;

			this.Notified = false;

			akashiCheck = (firstShip == 182 || firstShip == 187);
			if (akashiCheck == true)
				this.BaseTime = DateTimeOffset.Now;
		}

		public void Reset()
		{
			if (this.Elapsed >= 20)
			{
				this.BaseTime = DateTimeOffset.Now;
				this.Notified = false;
			}
		}

		protected override void Tick()
		{
			base.Tick();

			this.TimerTick?.Invoke(this, new EventArgs());
			if (!Notified && this.Elapsed >= 20)
			{
				this.Notified = true;
				this.Repaired?.Invoke(this, new EventArgs());
			}
		}
	}
}
