using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.ViewModels.Fleets;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class KanColleMonitorViewModel : ViewModel
	{
		private Volume volume;

		public HomeportViewModel Homeport { get; private set; }

		#region IsMute 変更通知プロパティ

		private bool _IsMute;

		public bool IsMute
		{
			get { return this._IsMute; }
			set
			{
				if (this._IsMute != value)
				{
					this._IsMute = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public KanColleMonitorViewModel()
		{
			this.Homeport = new HomeportViewModel();

			this.CreateVolumeInstanceIfNull();
		}

		public void TakeScreenshot()
		{
			var message = new ScreenshotMessage("Screenshot/Save")
			{
				Path = Helper.CreateScreenshotFilePath(),
			};

			this.Messenger.Raise(message);
		}

		public void ToggleMute()
		{
			if (this.CreateVolumeInstanceIfNull())
			{
				this.volume.ToggleMute();
			}
		}


		private bool CreateVolumeInstanceIfNull()
		{
			if (this.volume == null)
			{
				try
				{
					this.volume = Volume.GetInstance();
					this.CompositeDisposable.Add(new PropertyChangedEventListener(this.volume)
					{
						{ "IsMute", (sender, args) => this.IsMute = this.volume.IsMute },
					});
					this.IsMute = this.volume.IsMute;
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
					return false;
				}
			}

			return true;
		}
	}
}
