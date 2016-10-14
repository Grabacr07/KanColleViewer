using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reactive.Linq;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class AkashiTimerViewModel : ViewModel
	{
		private AkashiTimer source;

		#region TimerVisibility 변경 통지 프로퍼티
		private Visibility _TimerVisibility;

		public Visibility TimerVisibility
		{
			get { return _TimerVisibility; }
			set
			{
				if(_TimerVisibility != value)
				{
					_TimerVisibility = value;
					this.RaisePropertyChanged();
				}
			}
		}
        #endregion

        private bool Available => this.source.Available;
        private TimeSpan ElapsedTime => TimeSpan.FromTicks(DateTime.Now.Ticks) - this.source.BaseTime;
        public string CurrentTime => Available
            ? $"{(int)ElapsedTime.TotalHours:D2}:{ElapsedTime.ToString(@"mm\:ss")}"
            : "--:--:--";

		public AkashiTimerViewModel()
		{
			this.source = new AkashiTimer();

            this.source.TimerTick += this.Tick;
            this.CompositeDisposable.Add(() => this.source.TimerTick -= this.Tick);

            NotifyService.Current.UpdateAkashiTimer(this.source);

            KanColleClient.Current.Proxy.api_req_hensei_change.TryParse()
                .Where(x => x.IsSuccess)
                .Subscribe(x => this.Update(
                    int.Parse(x.Request["api_id"]),
                    int.Parse(x.Request["api_ship_idx"]),
                    int.Parse(x.Request["api_ship_id"])
                ));

			KanColleClient.Current.Proxy.api_port.TryParse().Subscribe(x => this.Reset());
            KanColleClient.Current.Proxy.api_req_hensei_preset_select.TryParse()
                .Where(x => x.IsSuccess)
                .Subscribe(x => this.Update(
                    int.Parse(x.Request["api_deck_id"])
                ));

            KanColleSettings.UseRepairTimer.ValueChanged += (s, e) => this.UpdateVisibility();
            this.UpdateVisibility();
        }

        private void Tick(object sender, EventArgs args)
        {
            this.RaisePropertyChanged(nameof(CurrentTime));
        }

        private void UpdateVisibility()
		{
			if (KanColleSettings.UseRepairTimer)
				TimerVisibility = Visibility.Visible;
			else
				TimerVisibility = Visibility.Collapsed;
		}

		public void Update(int fleetId, int shipIdx, int shipId)
		{
            source.Update(fleetId, shipIdx, shipId);
        }
        public void Update(int fleetId)
        {
            source.Update(fleetId);
        }

        public void Reset()
		{
			source.Reset();
		}
	}
}
