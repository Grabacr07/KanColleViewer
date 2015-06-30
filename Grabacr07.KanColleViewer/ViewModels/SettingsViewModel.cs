using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Composition;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleWrapper.Models;
using Livet;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class SettingsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return Resources.Settings; }
			protected set { throw new NotImplementedException(); }
		}

		#region ScreenshotFolder 変更通知プロパティ

		public string ScreenshotFolder
		{
			get { return Settings.Current.ScreenshotFolder; }
			set
			{
				if (Settings.Current.ScreenshotFolder != value)
				{
					Settings.Current.ScreenshotFolder = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.CanOpenScreenshotFolder));
				}
			}
		}

		#endregion

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder => Directory.Exists(this.ScreenshotFolder);

		#endregion

		#region ScreenshotImageFormat 変更通知プロパティ

		public SupportedImageFormat ScreenshotImageFormat
		{
			get { return Settings.Current.ScreenshotImageFormat; }
			set
			{
				if (Settings.Current.ScreenshotImageFormat != value)
				{
					Settings.Current.ScreenshotImageFormat = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Libraries 変更通知プロパティ

		private IEnumerable<BindableTextViewModel> _Libraries;

		public IEnumerable<BindableTextViewModel> Libraries
		{
			get { return this._Libraries; }
			set
			{
				if (!Equals(this._Libraries, value))
				{
					this._Libraries = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region IsDarkTheme 変更通知プロパティ

		private bool _IsDarkTheme;

		public bool IsDarkTheme
		{
			get { return this._IsDarkTheme; }
			set
			{
				if (this._IsDarkTheme != value)
				{
					this._IsDarkTheme = value;
					this.RaisePropertyChanged();
					if (value) ThemeService.Current.ChangeTheme(Theme.Dark);
				}
			}
		}

		#endregion

		#region IsLightTheme 変更通知プロパティ

		private bool _IsLightTheme;

		public bool IsLightTheme
		{
			get { return this._IsLightTheme; }
			set
			{
				if (this._IsLightTheme != value)
				{
					this._IsLightTheme = value;
					this.RaisePropertyChanged();
					if (value) ThemeService.Current.ChangeTheme(Theme.Light);
				}
			}
		}

		#endregion

		#region Cultures 変更通知プロパティ

		private IReadOnlyCollection<CultureViewModel> _Cultures;

		public IReadOnlyCollection<CultureViewModel> Cultures
		{
			get { return this._Cultures; }
			set
			{
				if (!Equals(this._Cultures, value))
				{
					this._Cultures = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Culture 変更通知プロパティ

		/// <summary>
		/// カルチャを取得または設定します。
		/// </summary>
		public string Culture
		{
			get { return Settings.Current.Culture; }
			set
			{
				if (Settings.Current.Culture != value)
				{
					ResourceService.Current.ChangeCulture(value);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BrowserZoomFactor 変更通知プロパティ

		private BrowserZoomFactor _BrowserZoomFactor;

		public BrowserZoomFactor BrowserZoomFactor
		{
			get { return this._BrowserZoomFactor; }
			private set
			{
				if (this._BrowserZoomFactor != value)
				{
					this._BrowserZoomFactor = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifierPlugins 変更通知プロパティ

		private List<NotifierViewModel> _NotifierPlugins;

		public List<NotifierViewModel> NotifierPlugins
		{
			get { return this._NotifierPlugins; }
			set
			{
				if (this._NotifierPlugins != value)
				{
					this._NotifierPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region AllPlugins 変更通知プロパティ

		private List<PluginViewModel> _AllPlugins;

		public List<PluginViewModel> AllPlugins
		{
			get { return this._AllPlugins; }
			set
			{
				if (this._AllPlugins != value)
				{
					this._AllPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BlacklistedPlugins 変更通知プロパティ

		private ReadOnlyDispatcherCollection<BlacklistedPluginViewModel> _BlacklistedPlugins;

		public ReadOnlyDispatcherCollection<BlacklistedPluginViewModel> BlacklistedPlugins
		{
			get { return this._BlacklistedPlugins; }
			set
			{
				if (this._BlacklistedPlugins != value)
				{
					this._BlacklistedPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		#region ViewRangeSettingsCollection 変更通知プロパティ

		private List<ViewRangeSettingsViewModel> _ViewRangeSettingsCollection;

		public List<ViewRangeSettingsViewModel> ViewRangeSettingsCollection
		{
			get { return this._ViewRangeSettingsCollection; }
			set
			{
				if (this._ViewRangeSettingsCollection != value)
				{
					this._ViewRangeSettingsCollection = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.Libraries = ProductInfo.Libraries.Aggregate(
				new List<BindableTextViewModel>(),
				(list, lib) =>
				{
					list.Add(new BindableTextViewModel { Text = list.Count == 0 ? "Build with " : ", " });
					list.Add(new HyperlinkViewModel { Text = lib.Name.Replace(' ', Convert.ToChar(160)), Uri = lib.Url });
					// プロダクト名の途中で改行されないように、space を non-break space に置き換えてあげてるんだからねっっ
					return list;
				});

			this.Cultures = new[] { new CultureViewModel { DisplayName = "(auto)" } }
				.Concat(ResourceService.Current.SupportedCultures
					.Select(x => new CultureViewModel { DisplayName = x.EnglishName, Name = x.Name })
					.OrderBy(x => x.DisplayName))
				.ToList();

			this.CompositeDisposable.Add(new PropertyChangedEventListener(Settings.Current)
			{
				(sender, args) => this.RaisePropertyChanged(args.PropertyName),
			});

			this._IsDarkTheme = ThemeService.Current.Theme == Theme.Dark;
			this._IsLightTheme = ThemeService.Current.Theme == Theme.Light;

			var zoomFactor = new BrowserZoomFactor { Current = Settings.Current.BrowserZoomFactor };
			this.CompositeDisposable.Add(new PropertyChangedEventListener(zoomFactor)
			{
				{ nameof(zoomFactor.Current), (sender, args) => Settings.Current.BrowserZoomFactor = zoomFactor.Current },
			});
			this.BrowserZoomFactor = zoomFactor;

			this.ViewRangeSettingsCollection = ViewRangeCalcLogic.Logics
				.Select(x => new ViewRangeSettingsViewModel(x))
				.ToList();

			this.ReloadPlugins();
		}


		public void OpenScreenshotFolderSelectionDialog()
		{
			var message = new FolderSelectionMessage("OpenFolderDialog/Screenshot")
			{
				Title = Resources.Settings_Screenshot_FolderSelectionDialog_Title,
				DialogPreference = Helper.IsWindows8OrGreater
					? FolderSelectionDialogPreference.CommonItemDialog
					: FolderSelectionDialogPreference.FolderBrowser,
				SelectedPath = this.CanOpenScreenshotFolder
					? this.ScreenshotFolder
					: ""
			};
			this.Messenger.Raise(message);

			if (Directory.Exists(message.Response))
			{
				this.ScreenshotFolder = message.Response;
			}
		}

		public void OpenScreenshotFolder()
		{
			if (this.CanOpenScreenshotFolder)
			{
				try
				{
					Process.Start(this.ScreenshotFolder);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
		}

		public void ClearZoomFactor()
		{
			Application.Current.MainWindowViewModel.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser.Zoom" });
		}

		public void SetLocationLeft()
		{
			Application.Current.MainWindowViewModel.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window.Location", Left = 0.0 });
		}


		public void ReloadPlugins()
		{
			this.NotifierPlugins = new List<NotifierViewModel>();

			this.AllPlugins = new List<PluginViewModel>(
				PluginHost.Instance.Plugins.Select(x => new PluginViewModel(x)));

			var collection = ViewModelHelper.CreateReadOnlyDispatcherCollection(
				Settings.Current.BlacklistedPlugins,
				x => new BlacklistedPluginViewModel(x),
				DispatcherHelper.UIDispatcher);
			this.CompositeDisposable.Add(collection);
			this.BlacklistedPlugins = collection;
		}


		public class ViewRangeSettingsViewModel
		{
			private bool selected;

			public ICalcViewRange Logic { get; set; }

			public bool Selected
			{
				get { return this.selected; }
				set
				{
					this.selected = value;
					if (value)
					{
						Settings.Current.KanColleClientSettings.ViewRangeCalcType = this.Logic.Id;
					}
				}
			}

			public ViewRangeSettingsViewModel(ICalcViewRange logic)
			{
				this.Logic = logic;
				this.selected = Settings.Current.KanColleClientSettings.ViewRangeCalcType == logic.Id;
			}
		}
	}
}
