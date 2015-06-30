using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using MetroRadiance;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainWindowViewModel : WindowViewModel
	{
		private Mode currentMode;
		private MainContentViewModel mainContent;

		public NavigatorViewModel Navigator { get; private set; }

		public SettingsViewModel Settings { get; private set; }

		#region Mode 変更通知プロパティ

		public Mode Mode
		{
			get { return this.currentMode; }
			set
			{
				this.currentMode = value;
				switch (value)
				{
					case Mode.NotStarted:
						this.Content = StartContentViewModel.Instance;
						this.StatusBar = StartContentViewModel.Instance;
						StatusService.Current.Set(Resources.StatusBar_NotStarted);
						ThemeService.Current.ChangeAccent(Accent.Purple);
						break;
					case Mode.Started:
						this.Content = this.mainContent ?? (this.mainContent = new MainContentViewModel());
						StatusService.Current.Set(Resources.StatusBar_Ready);
						ThemeService.Current.ChangeAccent(Accent.Blue);
						break;
					case Mode.InSortie:
						ThemeService.Current.ChangeAccent(Accent.Orange);
						break;
				}

				this.RaisePropertyChanged();
			}
		}

		#endregion

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

		#region StatusMessage 変更通知プロパティ

		private string _StatusMessage;

		public string StatusMessage
		{
			get { return this._StatusMessage; }
			set
			{
				if (this._StatusMessage != value)
				{
					this._StatusMessage = value;
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

		public override sealed bool CanClose => base.CanClose
												|| Models.Settings.Current.CanCloseWithoutConfirmation      // 設定で「確認なしで終了」が有効なら終了できる
												|| Application.Current.State != ApplicationState.Running; // アプリケーションが起動中か終了処理中なら終了できる

		public MainWindowViewModel()
		{
			this.Title = ProductInfo.Title;
			this.CanClose = false;

			this.Navigator = new NavigatorViewModel();
			this.Settings = new SettingsViewModel();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(StatusService.Current)
			{
				{ nameof(StatusService.Message), (sender, args) => this.StatusMessage = StatusService.Current.Message },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
			{
				{ nameof(KanColleClient.IsStarted), (sender, args) => this.UpdateMode() },
				{ nameof(KanColleClient.IsInSortie), (sender, args) => this.UpdateMode() },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(Models.Settings.Current)
			{
				{ nameof(Models.Settings.CanCloseWithoutConfirmation), (sender, args) => this.RaiseCanCloseChanged() },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(Application.Current)
			{
				{ nameof(Application.State), (sender, args) => this.RaiseCanCloseChanged() },
			});

			this.UpdateMode();
		}

		public void TakeScreenshot()
		{
			var path = Helper.CreateScreenshotFilePath();
			var message = new ScreenshotMessage("Screenshot.Save") { Path = path, };

			this.Messenger.Raise(message);

			var notify = message.Response.IsSuccess
				? Resources.Screenshot_Saved + Path.GetFileName(path)
				: Resources.Screenshot_Failed + message.Response.Exception.Message;
			StatusService.Current.Notify(notify);
		}

		public override void CloseCanceledCallback()
		{
			var dialog = new DialogViewModel { Title = "終了確認", };

			this.Transition(dialog, typeof(ExitDialog), TransitionMode.Modal);

			if (dialog.DialogResult)
			{
				this.CanClose = true;
				this.InvokeOnUIDispatcher(this.Close);
			}
		}

		private void RaiseCanCloseChanged()
		{
			this.RaisePropertyChanged(nameof(this.CanClose));
		}

		private void UpdateMode()
		{
			this.Mode = KanColleClient.Current.IsStarted
				? KanColleClient.Current.IsInSortie
					? Mode.InSortie
					: Mode.Started
				: Mode.NotStarted;
		}
	}
}
