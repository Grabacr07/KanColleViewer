using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Composition;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleWrapper;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.IO;
using MetroRadiance;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleViewer.Views.Controls;

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
					this.RaisePropertyChanged("CanOpenScreenshotFolder");
				}
			}
		}

		#endregion

		#region CanOpenScreenshotFolder 変更通知プロパティ

		public bool CanOpenScreenshotFolder
		{
			get { return Directory.Exists(this.ScreenshotFolder); }
		}

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

		#region EnableLogging 変更通知プロパティ

		public bool EnableLogging
		{
			get { return Settings.Current.KanColleClientSettings.EnableLogging; }
			set
			{
				if (Settings.Current.KanColleClientSettings.EnableLogging != value)
				{
					Settings.Current.KanColleClientSettings.EnableLogging = value;
					KanColleClient.Current.Logger.EnableLogging = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableTranslations 変更通知プロパティ

		public bool EnableTranslations
		{
			get { return Settings.Current.EnableTranslations; }
			set
			{
				if (Settings.Current.EnableTranslations != value)
				{
					Settings.Current.EnableTranslations = value;
					KanColleClient.Current.Translations.EnableTranslations = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableAddUntranslated 変更通知プロパティ

		public bool EnableAddUntranslated
		{
			get { return Settings.Current.EnableAddUntranslated; }
			set
			{
				if (Settings.Current.EnableAddUntranslated != value)
				{
					Settings.Current.EnableAddUntranslated = value;
					KanColleClient.Current.Translations.EnableAddUntranslated = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		
		#region EnableCriticalNotify 変更通知プロパティ

		public bool EnableCriticalNotify
		{
			get { return Settings.Current.EnableCriticalNotify; }
			set
			{
				if (Settings.Current.EnableCriticalNotify != value)
				{
					Settings.Current.EnableCriticalNotify = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableCriticalAccent 変更通知プロパティ

		public bool EnableCriticalAccent
		{
			get { return Settings.Current.EnableCriticalAccent; }
			set
			{
				if (Settings.Current.EnableCriticalAccent != value)
				{
					Settings.Current.EnableCriticalAccent = value;
					if (!Settings.Current.EnableCriticalAccent && App.ViewModelRoot.Mode == Mode.CriticalCondition)
						App.ViewModelRoot.Mode = Mode.Started;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableCriticalPopup 変更通知プロパティ

		public bool EnableCriticalPopup
		{
			get { return Settings.Current.EnableCriticalPopup; }
			set
			{
				if (Settings.Current.EnableCriticalPopup != value)
				{
					Settings.Current.EnableCriticalPopup = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		
		#region EquipmentVersion 変更通知プロパティ

		public string EquipmentVersion
		{
			get { return KanColleClient.Current.Translations.EquipmentVersion; }
			set
			{
				if (KanColleClient.Current.Translations.EquipmentVersion != value)
				{
					KanColleClient.Current.Translations.EquipmentVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region OperationsVersion 変更通知プロパティ

		public string OperationsVersion
		{
			get { return KanColleClient.Current.Translations.OperationsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.OperationsVersion != value)
				{
					KanColleClient.Current.Translations.OperationsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region QuestsVersion 変更通知プロパティ

		public string QuestsVersion
		{
			get { return KanColleClient.Current.Translations.QuestsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.QuestsVersion != value)
				{
					KanColleClient.Current.Translations.QuestsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipsVersion 変更通知プロパティ

		public string ShipsVersion
		{
			get { return KanColleClient.Current.Translations.ShipsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.ShipsVersion != value)
				{
					KanColleClient.Current.Translations.ShipsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ShipTypesVersion 変更通知プロパティ

		public string ShipTypesVersion
		{
			get { return KanColleClient.Current.Translations.ShipTypesVersion; }
			set
			{
				if (KanColleClient.Current.Translations.ShipTypesVersion != value)
				{
					KanColleClient.Current.Translations.ShipTypesVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ExpeditionsVersion 変更通知プロパティ

		public string ExpeditionsVersion
		{
			get { return KanColleClient.Current.Translations.ExpeditionsVersion; }
			set
			{
				if (KanColleClient.Current.Translations.ExpeditionsVersion != value)
				{
					KanColleClient.Current.Translations.ExpeditionsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region AppOnlineVersion 変更通知プロパティ

		private string _AppOnlineVersion;
		public string AppOnlineVersionURL { get; set; }

		public string AppOnlineVersion
		{
			get { return _AppOnlineVersion; }
			set
			{
				if (_AppOnlineVersion != value)
				{
					_AppOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("AppOnlineVersionURL");
				}
			}
		}

		#endregion

		#region EquipmentOnlineVersion 変更通知プロパティ

		private string _EquipmentOnlineVersion;
		public string EquipmentOnlineVersionURL { get; set; }

		public string EquipmentOnlineVersion
		{
			get { return _EquipmentOnlineVersion; }
			set
			{
				if (_EquipmentOnlineVersion != value)
				{
					_EquipmentOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("EquipmentOnlineVersionURL");
				}
			}
		}

		#endregion

		#region OperationsOnlineVersion 変更通知プロパティ

		private string _OperationsOnlineVersion;
		public string OperationsOnlineVersionURL { get; set; }

		public string OperationsOnlineVersion
		{
			get { return _OperationsOnlineVersion; }
			set
			{
				if (_OperationsOnlineVersion != value)
				{
					_OperationsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("OperationsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region QuestsOnlineVersion 変更通知プロパティ

		private string _QuestsOnlineVersion;
		public string QuestsOnlineVersionURL { get; set; }

		public string QuestsOnlineVersion
		{
			get { return _QuestsOnlineVersion; }
			set
			{
				if (_QuestsOnlineVersion != value)
				{
					_QuestsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("QuestsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region ShipsOnlineVersion 変更通知プロパティ

		private string _ShipsOnlineVersion;
		public string ShipsOnlineVersionURL { get; set; }

		public string ShipsOnlineVersion
		{
			get { return _ShipsOnlineVersion; }
			set
			{
				if (_ShipsOnlineVersion != value)
				{
					_ShipsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ShipsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region ShipTypesOnlineVersion 変更通知プロパティ

		private string _ShipTypesOnlineVersion;
		public string ShipTypesOnlineVersionURL { get; set; }

		public string ShipTypesOnlineVersion
		{
			get { return _ShipTypesOnlineVersion; }
			set
			{
				if (_ShipTypesOnlineVersion != value)
				{
					_ShipTypesOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ShipTypesOnlineVersionURL");
				}
			}
		}

		#endregion

		#region ExpeditionsOnlineVersion 変更通知プロパティ

		private string _ExpeditionsOnlineVersion;
		public string ExpeditionsOnlineVersionURL { get; set; }

		public string ExpeditionsOnlineVersion
		{
			get { return _ExpeditionsOnlineVersion; }
			set
			{
				if (_ExpeditionsOnlineVersion != value)
				{
					_ExpeditionsOnlineVersion = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged("ExpeditionsOnlineVersionURL");
				}
			}
		}

		#endregion

		#region EnableUpdateNotification 変更通知プロパティ

		public bool EnableUpdateNotification
		{
			get { return Settings.Current.EnableUpdateNotification; }
			set
			{
				if (Settings.Current.EnableUpdateNotification != value)
				{
					Settings.Current.EnableUpdateNotification = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableUpdateTransOnStart 変更通知プロパティ

		public bool EnableUpdateTransOnStart
		{
			get { return Settings.Current.EnableUpdateTransOnStart; }
			set
			{
				if (Settings.Current.EnableUpdateTransOnStart != value)
				{
					Settings.Current.EnableUpdateTransOnStart = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CustomSoundVolume 変更通知プロパティ

		public int CustomSoundVolume
		{
			get { return Settings.Current.CustomSoundVolume; }
			set
			{
				if (Settings.Current.CustomSoundVolume != value)
				{
					Settings.Current.CustomSoundVolume = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableResizing 変更通知プロパティ

		public bool EnableResizing
		{
			get { return Settings.Current.EnableResizing; }
			set
			{
				if (Settings.Current.EnableResizing != value)
				{
					Settings.Current.EnableResizing = value;
					KanColleHost.Current.EnableResizing = value;
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

		#region ToolPlugins 変更通知プロパティ

		private List<ToolViewModel> _ToolPlugins;

		public List<ToolViewModel> ToolPlugins
		{
			get { return this._ToolPlugins; }
			set
			{
				if (this._ToolPlugins != value)
				{
					this._ToolPlugins = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ViewRangeType1 変更通知プロパティ

		private bool _ViewRangeType1;

		public bool ViewRangeType1
		{
			get { return this._ViewRangeType1; }
			set
			{
				if (this._ViewRangeType1 != value)
				{
					this._ViewRangeType1 = value;
					this.RaisePropertyChanged();
					if (value) Settings.Current.KanColleClientSettings.ViewRangeCalcLogic = ViewRangeCalcLogic.Type1;
				}
			}
		}

		#endregion

		#region ViewRangeType2 変更通知プロパティ

		private bool _ViewRangeType2;

		public bool ViewRangeType2
		{
			get { return this._ViewRangeType2; }
			set
			{
				if (this._ViewRangeType2 != value)
				{
					this._ViewRangeType2 = value;
					this.RaisePropertyChanged();
					if (value) Settings.Current.KanColleClientSettings.ViewRangeCalcLogic = ViewRangeCalcLogic.Type2;
				}
			}
		}

		#endregion


		public SettingsViewModel()
		{
			if (Helper.IsInDesignMode) return;

			this.Libraries = App.ProductInfo.Libraries.Aggregate(
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
				{ "Current", (sender, args) => Settings.Current.BrowserZoomFactor = zoomFactor.Current },
			});
			this.BrowserZoomFactor = zoomFactor;

			this._ViewRangeType1 = Settings.Current.KanColleClientSettings.ViewRangeCalcLogic == ViewRangeCalcLogic.Type1;
			this._ViewRangeType2 = Settings.Current.KanColleClientSettings.ViewRangeCalcLogic == ViewRangeCalcLogic.Type2;

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
			App.ViewModelRoot.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser/Zoom" });
		}

		public void SetLocationLeft()
		{
			App.ViewModelRoot.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window/Location", Left = 0.0 });
		}


		public void ReloadPlugins()
		{
			this.NotifierPlugins = new List<NotifierViewModel>(PluginHost.Instance.Notifiers.Select(x => new NotifierViewModel(x)));
			this.ToolPlugins = new List<ToolViewModel>(PluginHost.Instance.Tools.Select(x => new ToolViewModel(x)));
		}
		public void CheckForUpdates()
		{
			if (KanColleClient.Current.Updater.LoadVersion(Properties.Settings.Default.KCVUpdateUrl.AbsoluteUri))
			{
				AppOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.App, true);
				EquipmentOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Equipment, true);
				OperationsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Operations, true);
				QuestsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Quests, true);
				ShipsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Ships, true);
				ShipTypesOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.ShipTypes, true);
				ExpeditionsOnlineVersionURL = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Expeditions, true);

				AppOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.App);
				EquipmentOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Equipment);
				OperationsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Operations);
				QuestsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Quests);
				ShipsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Ships);
				ShipTypesOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.ShipTypes);
				ExpeditionsOnlineVersion = KanColleClient.Current.Updater.GetOnlineVersion(TranslationType.Expeditions);
			}
			else
			{
				PluginHost.Instance.GetNotifier().Show(
					NotifyType.Update,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_CheckFailed,
					() => App.ViewModelRoot.Activate());
			}
		}

		public void UpdateTranslations()
		{
			int UpdateStatus = KanColleClient.Current.Updater.UpdateTranslations(Properties.Settings.Default.XMLTransUrl.AbsoluteUri, KanColleClient.Current.Translations);

			if (UpdateStatus > 0)
			{
				PluginHost.Instance.GetNotifier().Show(
					NotifyType.Update,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Success,
					() => App.ViewModelRoot.Activate());
			}
			else if (UpdateStatus < 0)
			{
				PluginHost.Instance.GetNotifier().Show(
					NotifyType.Update,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Fail,
					() => App.ViewModelRoot.Activate());
			}
			else
			{
				PluginHost.Instance.GetNotifier().Show(
					NotifyType.Update,
					Resources.Updater_Notification_Title,
					Resources.Updater_Notification_TransUpdate_Same,
					() => App.ViewModelRoot.Activate());
			}

		}
	}
}
