﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models.Data.Xml;
using Grabacr07.KanColleWrapper;
using Livet;

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

		public static Settings Current { get; set; }

		public static void Load()
		{
			try
			{
				Current = filePath.ReadXml<Settings>();
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
				ScreenshotFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
				ScreenshotFilename = "KanColle-{0:d04}.png",
				ScreenshotImageFormat = SupportedImageFormat.Png,
				CanDisplayBuildingShipName = false,
				LocalProxyPort = Properties.Settings.Default.DefaultLocalProxyPort,
				KanColleClientSettings = new KanColleClientSettings(),
			};
		}

		#endregion


		#region CanCloseWithoutConfirmation 変更通知プロパティ

		private bool _CanCloseWithoutConfirmation;

		/// <summary>
		/// メイン ウィンドウが確認なしで終了できるかどうかを示す値を取得または設定します。
		/// </summary>
		public bool CanCloseWithoutConfirmation
		{
			get { return this._CanCloseWithoutConfirmation; }
			set
			{
				if (this._CanCloseWithoutConfirmation != value)
				{
					this._CanCloseWithoutConfirmation = value;
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

		#endregion

		#region IsEnableChangeLocalProxyPort 変更通知プロパティ

		private bool _IsEnableChangeLocalProxyPort;

		public bool IsEnableChangeLocalProxyPort
		{
			get { return this._IsEnableChangeLocalProxyPort; }
			set
			{
				if (this._IsEnableChangeLocalProxyPort != value)
				{
					this._IsEnableChangeLocalProxyPort = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region LocalProxyPort 変更通知プロパティ

		private int _LocalProxyPort;

		public int LocalProxyPort
		{
			get { return this._LocalProxyPort; }
			set
			{
				if (this._LocalProxyPort != value)
				{
					this._LocalProxyPort = value;
					this.RaisePropertyChanged();
				}
			}
		}

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

		#region BlacklistedPlugins 変更通知プロパティ

		private ObservableSynchronizedCollection<BlacklistedPluginData> _BlacklistedPlugins = new ObservableSynchronizedCollection<BlacklistedPluginData>();

		/// <summary>
		/// ロードに失敗したプラグインのリストを取得または設定します。
		/// </summary>
		public ObservableSynchronizedCollection<BlacklistedPluginData> BlacklistedPlugins
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

		#region FailReboot 変更通知プロパティ

		private bool _FailReboot;

		public bool FailReboot
		{
			get { return this._FailReboot; }
			set
			{
				if (this._FailReboot != value)
				{
					this._FailReboot = value;
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