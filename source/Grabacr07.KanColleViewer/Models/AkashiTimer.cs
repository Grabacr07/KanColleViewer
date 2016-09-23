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

        private TimeSpan _BaseTime;

		public TimeSpan BaseTime
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

        #region Available 변경통지 프로퍼티

        private bool _Available;

        public bool Available
        {
            get { return _Available; }
            private set
            {
                if (_Available != value)
                {
                    _Available = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        #endregion

        private bool Notified { get; set; }
        public event EventHandler Repaired;
        public event EventHandler TimerTick;

        public AkashiTimer()
		{
			BaseTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
            Notified = false;
            Available = false;
        }

        public void Update(int fleetId, int shipIdx, int shipId)
		{
            if (shipIdx == -1) return;

			var akashiCheck = false;
			var fleets = KanColleClient.Current.Homeport.Organization.Fleets;
            var firstShip = fleets[fleetId].Ships[0]?.Info.Id ?? 0;

            Notified = false;
            Available = false;

            akashiCheck = (firstShip == 182 || firstShip == 187);
            if (shipIdx != -1 && akashiCheck == true)
            {
                BaseTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                Available = true;
            }
		}
        public void Update(int fleetId)
        {
            if (Available) return;

            var akashiCheck = false;
            var fleets = KanColleClient.Current.Homeport.Organization.Fleets;
            var firstShip = fleets[fleetId].Ships[0]?.Info.Id ?? 0;

            Notified = false;

            akashiCheck = (firstShip == 182 || firstShip == 187);
            if (akashiCheck == true)
            {
                BaseTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                Available = true;
            }
        }

        public void Reset()
        {
            foreach (var i in KanColleClient.Current.Homeport.Organization.Fleets.Keys)
                this.Update(i);

            if ((TimeSpan.FromTicks(DateTime.Now.Ticks) - BaseTime).Minutes >= 20)
            {
                BaseTime = TimeSpan.FromTicks(DateTime.Now.Ticks);
                Notified = false;
            }
        }

        protected override void Tick()
        {
            base.Tick();

            this.TimerTick?.Invoke(this, new EventArgs());
            if ((TimeSpan.FromTicks(DateTime.Now.Ticks) - BaseTime).TotalSeconds >= 20 * 60 && !Notified && Available)
            {
                this.Notified = true;
                this.Repaired?.Invoke(this, new EventArgs());
            }
        }
	}
}
