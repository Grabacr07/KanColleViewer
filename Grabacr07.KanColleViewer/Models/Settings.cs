using Grabacr07.KanColleViewer.Models.Data.Xml;
using Grabacr07.KanColleWrapper;
using Livet;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Grabacr07.KanColleViewer.Models
{
	[Serializable]
	public class Settings : NotificationObject
	{
		#region static members

		private static readonly string filePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"grabacr.net",
			"KanColleViewer",
			"Settings.xml");
		private static readonly string CurrentSettingsVersion = "1.4";
		public static Settings Current { get; set; }

		public static void Load()
		{
			try
			{
				Current = filePath.ReadXml<Settings>();
				if (Current.SettingsVersion != CurrentSettingsVersion)
					Current = GetInitialSettings();
			}
			catch (Exception ex)
			{
				Current = GetInitialSettings();
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}

		public static Settings GetInitialSettings()
		{
			return new Settings
			{
				SettingsVersion = CurrentSettingsVersion,
				ScreenshotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				ScreenshotFilename = "KanColle-{0:d04}.png",
				ScreenshotImageFormat = SupportedImageFormat.Png,
				CanDisplayBuildingShipName = false,
				EnableLogging = true,
				EnableTranslations = true,
				EnableAddUntranslated = true,
				EnableCriticalNotify = true,
				EnableCriticalAccent = true,
				EnableCriticalPopup = true,
				EnableUpdateNotification = true,
				EnableUpdateTransOnStart = true,
				NotifyBuildingCompleted = true,
				NotifyRepairingCompleted = true,
				NotifyExpeditionReturned = true,
				CustomSoundVolume = 99,
				EnableResizing = true,
				EnableEventMapInfo = true,
				EnableBattlePreview=true,
				KanColleClientSettings = new KanColleClientSettings(),
			};
		}

		#endregion

		#region SettingsVersion 変更通知プロパティ

		private string _SettingsVersion;

		public string SettingsVersion
		{
			get { return this._SettingsVersion; }
			set
			{
				if (this._SettingsVersion != value)
				{
					this._SettingsVersion = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region ScreenshotFolder 変更通知プロパティ

		private string _ScreenshotFolder;

		/// <summary>
		/// スクリーンショットの保存先フォルダーを取得または設定します。
		/// </summary>
		public string ScreenshotFolder
		{
			get { return this._ScreenshotFolder; }
			set
			{
				if (this._ScreenshotFolder != value)
				{
					this._ScreenshotFolder = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ScreenshotFilename 変更通知プロパティ

		private string _ScreenshotFilename;

		/// <summary>
		/// スクリーンショットのファイル名を取得または設定します。
		/// </summary>
		public string ScreenshotFilename
		{
			get { return this._ScreenshotFilename; }
			set
			{
				if (this._ScreenshotFilename != value)
				{
					this._ScreenshotFilename = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ScreenshotImageFormat 変更通知プロパティ

		private SupportedImageFormat _ScreenshotImageFormat;

		/// <summary>
		/// スクリーンショットのイメージ形式を取得または設定します。
		/// </summary>
		public SupportedImageFormat ScreenshotImageFormat
		{
			get
			{
				switch (this._ScreenshotImageFormat)
				{
					case SupportedImageFormat.Png:
					case SupportedImageFormat.Jpeg:
						break;
					default:
						this._ScreenshotImageFormat = SupportedImageFormat.Png;
						break;
				}
				return this._ScreenshotImageFormat;
			}
			set
			{
				if (this._ScreenshotImageFormat != value)
				{
					this._ScreenshotImageFormat = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CanDisplayBuildingShipName 変更通知プロパティ

		private bool _CanDisplayBuildingShipName;

		/// <summary>
		/// 建造中の艦の名前を表示するかどうかを示す値を取得または設定します。
		/// </summary>
		public bool CanDisplayBuildingShipName
		{
			get { return this._CanDisplayBuildingShipName; }
			set
			{
				if (this._CanDisplayBuildingShipName != value)
				{
					this._CanDisplayBuildingShipName = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyBuildingComplete 変更通知プロパティ

		private bool _NotifyBuildingCompleted;

		public bool NotifyBuildingCompleted
		{
			get { return this._NotifyBuildingCompleted; }
			set
			{
				if (this._NotifyBuildingCompleted != value)
				{
					this._NotifyBuildingCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyExpeditionReturned 変更通知プロパティ

		private bool _NotifyExpeditionReturned;

		public bool NotifyExpeditionReturned
		{
			get { return this._NotifyExpeditionReturned; }
			set
			{
				if (this._NotifyExpeditionReturned != value)
				{
					this._NotifyExpeditionReturned = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyRepairingCompleted 変更通知プロパティ

		private bool _NotifyRepairingCompleted;

		public bool NotifyRepairingCompleted
		{
			get { return this._NotifyRepairingCompleted; }
			set
			{
				if (this._NotifyRepairingCompleted != value)
				{
					this._NotifyRepairingCompleted = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotifyFleetRejuvenated 変更通知プロパティ

		private bool _NotifyFleetRejuvenated;

		public bool NotifyFleetRejuvenated
		{
			get { return this._NotifyFleetRejuvenated; }
			set
			{
				if (this._NotifyFleetRejuvenated != value)
				{
					this._NotifyFleetRejuvenated = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ProxySettings 変更通知プロパティ

		private ProxySettings _ProxySettings;

		public ProxySettings ProxySettings
		{
			get { return this._ProxySettings ?? (this._ProxySettings = new ProxySettings()); }
			set
			{
				if (this._ProxySettings != value)
				{
					this._ProxySettings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#region old properties

		public bool EnableProxy
		{
			get { return this.ProxySettings.IsEnabled; }
			set { this.ProxySettings.IsEnabled = value; }
		}

		public bool EnableSSLProxy
		{
			get { return this.ProxySettings.IsEnabledOnSSL; }
			set { this.ProxySettings.IsEnabledOnSSL = value; }
		}

		public string ProxyHost
		{
			get { return this.ProxySettings.Host; }
			set { this.ProxySettings.Host = value; }
		}
		
		public ushort ProxyPort
		{
			get { return this.ProxySettings.Port; }
			set { this.ProxySettings.Port = value; }
		}

		#endregion

		#endregion

		#region TopMost 変更通知プロパティ

		private bool _TopMost;

		/// <summary>
		/// メイン ウィンドウを常に最前面に表示するかどうかを示す値を取得または設定します。
		/// </summary>
		public bool TopMost
		{
			get { return this._TopMost; }
			set
			{
				if (this._TopMost != value)
				{
					this._TopMost = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Culture 変更通知プロパティ

		private string _Culture;

		/// <summary>
		/// カルチャを取得または設定します。
		/// </summary>
		public string Culture
		{
			get { return this._Culture; }
			set
			{
				if (this._Culture != value)
				{
					this._Culture = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BrowserZoomFactor 変更通知プロパティ

		private int _BrowserZoomFactorPercentage = 100;
		private double? _BrowserZoomFactor;

		/// <summary>
		/// ブラウザーの拡大率 (パーセンテージ) を取得または設定します。
		/// </summary>
		public int BrowserZoomFactorPercentage
		{
			get { return this._BrowserZoomFactorPercentage; }
			set { this._BrowserZoomFactorPercentage = value; }
		}

		/// <summary>
		/// ブラウザーの拡大率を取得または設定します。
		/// </summary>
		[XmlIgnore]
		public double BrowserZoomFactor
		{
			get { return this._BrowserZoomFactor ?? (this._BrowserZoomFactor = this.BrowserZoomFactorPercentage / 100.0).Value; }
			set
			{
				if (this._BrowserZoomFactor != value)
				{
					this._BrowserZoomFactor = value;
					this._BrowserZoomFactorPercentage = (int)(value * 100);
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region KanColleClientSettings 変更通知プロパティ

		public KanColleClientSettings KanColleClientSettings
		{
			get { return KanColleClient.Current.Settings; }
			set
			{
				if (KanColleClient.Current.Settings != value)
				{
					KanColleClient.Current.Settings = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableLogging 変更通知プロパティ

		private bool _EnableLogging;

		public bool EnableLogging
		{
			get { return this._EnableLogging; }
			set
			{
				if (this._EnableLogging != value)
				{
					this._EnableLogging = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableTranslations 変更通知プロパティ

		private bool _EnableTranslations;

		public bool EnableTranslations
		{
			get { return this._EnableTranslations; }
			set
			{
				if (this._EnableTranslations != value)
				{
					this._EnableTranslations = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region EnableResizing 変更通知プロパティ

		private bool _EnableResizing;

		public bool EnableResizing
		{
			get { return this._EnableResizing; }
			set
			{
				if (this._EnableResizing != value)
				{
					this._EnableResizing = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableEventMapInfo 変更通知プロパティ

		private bool _EnableEventMapInfo;

		public bool EnableEventMapInfo
		{
			get { return this._EnableEventMapInfo; }
			set
			{
				if (this._EnableEventMapInfo != value)
				{
					this._EnableEventMapInfo = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region CustomSoundVolume 変更通知プロパティ

		private int _CustomSoundVolume;

		public int CustomSoundVolume
		{
			get { return this._CustomSoundVolume; }
			set
			{
				if (this._CustomSoundVolume != value)
				{
					this._CustomSoundVolume = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region EnableAddUntranslated 変更通知プロパティ

		private bool _EnableAddUntranslated;

		public bool EnableAddUntranslated
		{
			get { return this._EnableAddUntranslated; }
			set
			{
				if (this._EnableAddUntranslated != value)
				{
					this._EnableAddUntranslated = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		#region EnableCriticalNotify 変更通知プロパティ

		private bool _EnableCriticalNotify;

		public bool EnableCriticalNotify
		{
			get { return this._EnableCriticalNotify; }
			set
			{
				if (this._EnableCriticalNotify != value)
				{
					this._EnableCriticalNotify = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableCriticalAccent 変更通知プロパティ

		private bool _EnableCriticalAccent;

		public bool EnableCriticalAccent
		{
			get { return this._EnableCriticalAccent; }
			set
			{
				if (this._EnableCriticalAccent != value)
				{
					this._EnableCriticalAccent = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableCriticalPopup 変更通知プロパティ

		private bool _EnableCriticalPopup;

		public bool EnableCriticalPopup
		{
			get { return this._EnableCriticalPopup; }
			set
			{
				if (this._EnableCriticalPopup != value)
				{
					this._EnableCriticalPopup = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableBattlePreview 変更通知プロパティ

		private bool _EnableBattlePreview;

		public bool EnableBattlePreview
		{
			get { return this._EnableBattlePreview; }
			set
			{
				if (this._EnableBattlePreview != value)
				{
					this._EnableBattlePreview = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableUpdateNotification 変更通知プロパティ

		private bool _EnableUpdateNotification;

		public bool EnableUpdateNotification
		{
			get { return this._EnableUpdateNotification; }
			set
			{
				if (this._EnableUpdateNotification != value)
				{
					this._EnableUpdateNotification = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region EnableUpdateTransOnStart 変更通知プロパティ

		private bool _EnableUpdateTransOnStart;

		public bool EnableUpdateTransOnStart
		{
			get { return this._EnableUpdateTransOnStart; }
			set
			{
				if (this._EnableUpdateTransOnStart != value)
				{
					this._EnableUpdateTransOnStart = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		public void Save()
		{
			try
			{
				this.WriteXml(filePath);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex);
			}
		}
	}
}
