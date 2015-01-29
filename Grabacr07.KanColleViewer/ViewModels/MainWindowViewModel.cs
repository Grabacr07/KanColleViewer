using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels.Composition;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.Windows;
using MetroRadiance;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainWindowViewModel : WindowViewModel
	{
		private Mode currentMode;
		private MainContentViewModel mainContent;

		public NavigatorViewModel Navigator { get; private set; }
		public SettingsViewModel Settings { get; private set; }
		#region RefreshNavigator

		private ICommand _RefreshNavigator;
		public ICommand RefreshNavigator
		{
			get { return _RefreshNavigator; }
		}

		#endregion

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
						StatusService.Current.Set(Properties.Resources.StatusBar_NotStarted);
						ThemeService.Current.ChangeAccent(Accent.Purple);
						break;
					case Mode.Started:
						this.Content = this.mainContent ?? (this.mainContent = new MainContentViewModel());
						StatusService.Current.Set(Properties.Resources.StatusBar_Ready);
						ThemeService.Current.ChangeTheme(Theme.Dark);
						ThemeService.Current.ChangeAccent(Accent.Blue);
						break;
					case Mode.InSortie:
						ThemeService.Current.ChangeAccent(Accent.Orange);
						break;
					case Mode.CriticalCondition:
						ThemeService.Current.ChangeAccent(Accent.Red);
						ThemeService.Current.ChangeTheme(Theme.CriticalRed);
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

		#region TopMost 変更通知プロパティ

		public bool TopMost
		{
			get { return Models.Settings.Current.TopMost; }
			set
			{
				if (Models.Settings.Current.TopMost != value)
				{
					Models.Settings.Current.TopMost = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CheckPreviewBattle 変更通知プロパティ

		private bool _CheckPreviewBattle;

		public bool CheckPreviewBattle
		{
			get { return this._CheckPreviewBattle; }
			set
			{
				if (this._CheckPreviewBattle != value)
				{
					this._CheckPreviewBattle = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region PreviewBattleVisible 変更通知プロパティ

		private Visibility _PreviewBattleVisible;

		public Visibility PreviewBattleVisible
		{
			get { return this._PreviewBattleVisible; }
			set
			{
				if (this._PreviewBattleVisible != value)
				{
					this._PreviewBattleVisible = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Items 変更通知プロパティ

		private List<ToolViewModel> _Tools;

		public List<ToolViewModel> Tools
		{
			get { return this._Tools; }
			set
			{
				if (this._Tools != value)
				{
					this._Tools = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion



		public MainWindowViewModel()
		{
			this.Title = App.ProductInfo.Title;
			this.Navigator = new NavigatorViewModel();
			this.Settings = new SettingsViewModel();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(StatusService.Current)
			{
				{ () => StatusService.Current.Message, (sender, args) => this.StatusMessage = StatusService.Current.Message },
			});
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
			{
				{ () => KanColleClient.Current.IsStarted, (sender, args) => this.UpdateMode() },
				{ () => KanColleClient.Current.IsInSortie, (sender, args) => this.UpdateMode() },
			});

			this.UpdateMode();

			this.Tools = new List<ToolViewModel>(PluginHost.Instance.Tools.Select(x => new ToolViewModel(x)));
			if (Tools.Any(x => x.ToolName == "전투예보"))
			{
				this.PreviewBattleVisible = Visibility.Collapsed;
				this.CheckPreviewBattle = false;
			}
			else
			{
				this.CheckPreviewBattle = KanColleClient.Current.PreviewBattle.EnableBattlePreview;
				if (this.CheckPreviewBattle) this.PreviewBattleVisible = Visibility.Visible;
				else this.PreviewBattleVisible = Visibility.Collapsed;

			}

			_RefreshNavigator = new RelayCommand(Navigator.ReNavigate);
		}

		public void TakeScreenshot()
		{
			var path = Helper.CreateScreenshotFilePath();
			var message = new ScreenshotMessage("Screenshot/Save") { Path = path, };

			this.Messenger.Raise(message);

			var notify = message.Response.IsSuccess
				? Properties.Resources.Screenshot_Saved + Path.GetFileName(path)
				: Properties.Resources.Screenshot_Failed + message.Response.Exception.Message;
			StatusService.Current.Notify(notify);
		}


		/// <summary>
		/// メイン ウィンドウをアクティブ化することを試みます。
		/// </summary>
		public void Activate()
		{
			this.Messenger.Raise(new WindowActionMessage(WindowAction.Active, "Window/Activate"));
		}


		private void UpdateMode()
		{
			this.Mode = KanColleClient.Current.IsStarted
				? KanColleClient.Current.IsInSortie
					? Mode.InSortie
					: Mode.Started
				: Mode.NotStarted;
		}

		public void ShowPreviewPopUp()
		{
			var window = new BattlePreviewsPopUpViewModel();
			var message = new TransitionMessage(window, "Show/BattlePreviewsPopup");
			this.Messenger.RaiseAsync(message);
		}
		public void ShowRefreshPopup()
		{
			var window = new RefreshPopupViewModel();
			var message = new TransitionMessage(window, "Show/RefreshPopup");
			this.Messenger.RaiseAsync(message);
		}
	}
}
