using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.ViewModels.Contents;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainWindowViewModel : WindowViewModel
	{
		public NavigatorViewModel Navigator { get; private set; }
		public VolumeViewModel Volume { get; private set; }

		#region Content 変更通知プロパティ

		private ViewModel _Content;

		public ViewModel Content
		{
			get { return this._Content; }
			set
			{
				if (this._Content != value)
				{
					this._Content = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region StatusBar 変更通知プロパティ

		private ViewModel _StatusBar;

		public ViewModel StatusBar
		{
			get { return this._StatusBar; }
			set
			{
				if (this._StatusBar != value)
				{
					this._StatusBar = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public MainWindowViewModel()
		{
			this.Title = "提督業も忙しい！";
			this.Navigator = new NavigatorViewModel();
			this.Volume = new VolumeViewModel();
			this.Content = NotStartedViewModel.Instance;

			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
			{
				{ "IsStarted", (sender, args) => this.Content = KanColleClient.Current.IsStarted ? new MainContentViewModel() : NotStartedViewModel.Instance as ViewModel },
			});
		}

		public void TakeScreenshot()
		{
			var message = new ScreenshotMessage("Screenshot/Save")
			{
				Path = Helper.CreateScreenshotFilePath(),
			};

			this.Messenger.Raise(message);
		}
	}
}
