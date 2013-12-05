using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Model;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : ViewModel
	{
		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set { Settings.Current.ScreenshotFolder = value; }
		}

		public string ProxyHost
		{
			get { return Settings.Current.ProxyHost; }
			set { Settings.Current.ProxyHost = value; }
		}

		public string ProxyPort
		{
			get { return Settings.Current.ProxyPort.ToString(); }
			set
			{
				UInt16 NumberPort;
				if (UInt16.TryParse(value, out NumberPort))
				{
					Settings.Current.ProxyPort = NumberPort;
				}
			}
		}

		public SettingsViewModel()
		{
			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});
		}
	}
}
