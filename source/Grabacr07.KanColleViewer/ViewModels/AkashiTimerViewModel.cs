using Grabacr07.KanColleViewer.Models;
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

		public string CurrentTime => $"{(int)this.source.CurrentTime.TotalHours:D2}:{this.source.CurrentTime.ToString(@"mm\:ss")}";

		public AkashiTimerViewModel()
		{
			this.source = new AkashiTimer();
			this.CompositeDisposable.Add(new PropertyChangedEventListener(source, (sender, args) => this.RaisePropertyChanged(args.PropertyName)));

			KanColleClient.Current.Proxy.api_req_hensei_change.TryParse().Subscribe(x => this.Update(x.Request));
			KanColleClient.Current.Proxy.api_port.TryParse().Subscribe(x => this.Reset());
		}

		private void UpdateVisibility()
		{
			if (KanColleClient.Current.Settings.UseRepairTimer)
				TimerVisibility = Visibility.Visible;
			else
				TimerVisibility = Visibility.Collapsed;
		}

		public void Update(NameValueCollection request)
		{
			UpdateVisibility();
			source.Update(request);
		}

		public void Reset()
		{
			UpdateVisibility();
			source.Reset();
		}
	}
}
