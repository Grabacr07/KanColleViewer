using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Microsoft.Win32;
using Livet;
using MetroRadiance;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using System.Net;

namespace Grabacr07.KanColleViewer
{
	public partial class App
	{
		public static ProductInfo ProductInfo { get; private set; }
		public static MainWindowViewModel ViewModelRoot { get; private set; }
		private bool IsUpdate { get; set; }
		static App()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception);

			DispatcherHelper.UIDispatcher = this.Dispatcher;
			ProductInfo = new ProductInfo();

			Settings.Load();
			PluginHost.Instance.Initialize();
			NotifierHost.Instance.Initialize(KanColleClient.Current);
			Helper.SetRegistryFeatureBrowserEmulation();
			if (Settings.Current.EnableMMCSS) Helper.SetMMCSSTask();

			KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
			KanColleClient.Current.Proxy.UpstreamProxySettings = Settings.Current.ProxySettings;

			ResourceService.Current.ChangeCulture(Settings.Current.Culture);
			ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);
			//Custom Settings
			//KanColleHost.Current.EnableResizing = Settings.Current.EnableResizing;
			KanColleClient.Current.Logger.EnableLogging = Settings.Current.EnableLogging;
			KanColleClient.Current.OracleOfCompass.EnableBattlePreview = Settings.Current.EnableBattlePreview;
			KanColleClient.Current.OracleOfCompass.IsBattleCalculated = false;
			KanColleClient.Current.OracleOfCompass.IsCompassCalculated = false;
			// Initialize translations
			KanColleClient.Current.Translations.EnableTranslations = Settings.Current.EnableTranslations;
			KanColleClient.Current.Translations.EnableAddUntranslated = Settings.Current.EnableAddUntranslated;
			// Update notification and download new translations (if enabled)
			if (KanColleClient.Current.Updater.LoadVersion(AppSettings.Default.KCVUpdateUrl.AbsoluteUri))
			{
				if (Settings.Current.EnableUpdateNotification && KanColleClient.Current.Updater.IsOnlineVersionGreater(0, ProductInfo.Version.ToString()))
				{
					string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
					if (File.Exists(Path.Combine(MainFolder, "AutoUpdater.exe")))
					{
						this.IsUpdate = true;
						Process MyProcess = new Process();
						MyProcess.StartInfo.FileName = "AutoUpdater.exe";
						MyProcess.StartInfo.WorkingDirectory = MainFolder;
						MyProcess.Start();
						MyProcess.Refresh();
					}
					else//AutoUpdater.exe가 없는 경우 알림만 띄운다.
					{
						PluginHost.Instance.GetNotifier().Show(
						NotifyType.Update,
						KanColleViewer.Properties.Resources.Updater_Notification_Title,
						string.Format(KanColleViewer.Properties.Resources.Updater_Notification_NewAppVersion, KanColleClient.Current.Updater.GetOnlineVersion(0)),
						() => App.ViewModelRoot.Activate());
					}
				}

				if (Settings.Current.EnableUpdateTransOnStart)
				{
					if (KanColleClient.Current.Updater.UpdateTranslations(AppSettings.Default.XMLTransUrl.AbsoluteUri, KanColleClient.Current.Translations) > 0)
					{
						PluginHost.Instance.GetNotifier().Show(
							NotifyType.Update,
							KanColleViewer.Properties.Resources.Updater_Notification_Title,
							KanColleViewer.Properties.Resources.Updater_Notification_TransUpdate_Success,
							() => App.ViewModelRoot.Activate());

					}
				}
			}


			ViewModelRoot = new MainWindowViewModel();
			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
			this.MainWindow.Show();

			RestoreWindowSize();
			// Check if Adobe Flash is installed in Microsoft Explorer
			//https://github.com/Yuubari/KanColleViewer/commit/d94a2c215122e4d03bf458f2a060b3a06f3c6599
			if (GetFlashVersion() == "")
			{
				MessageBoxResult MB = MessageBox.Show("Internet Explorer용 Flash Player ActiveX가 설치되어있지않습니다. 지금 설치하시겠습니까?", "Adobe Flash ActiveX를 찾을 수 없습니다" , MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);
				if (MB == MessageBoxResult.Yes)
				{
					Process.Start("IExplore.exe", @"http://get.adobe.com/flashplayer/");
					this.MainWindow.Close();
				}
			}


			if (this.IsUpdate) this.MainWindow.Close();
		}
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			KanColleClient.Current.Proxy.Shutdown();

			PluginHost.Instance.Dispose();
			Settings.Current.Save();
		}


		private static void ReportException(object sender, Exception exception)
		{
			#region const
			const string messageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			const string path = "error.log";
			#endregion

			try
			{
				var message = string.Format(messageFormat, DateTimeOffset.Now, sender, exception);

				Debug.WriteLine(message);
				File.AppendAllText(path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				KanColleClient.Current.CatchedErrorLogWriter.ReportException(ex.Source, ex);
			}
		}
		/// <summary>
		/// Obtains Adobe Flash Player's ActiveX element version.
		/// https://github.com/Yuubari/KanColleViewer/commit/d94a2c215122e4d03bf458f2a060b3a06f3c6599
		/// </summary>
		/// <returns>Empty string if Flash is not installed, otherwise the currently installed version.</returns>
		private static string GetFlashVersion()
		{
			string sVersion = "";

			RegistryKey FlashRK = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Macromedia\FlashPlayerActiveX");
			if (FlashRK != null)
			{
				sVersion = FlashRK.GetValue("Version", "").ToString();
			}

			return sVersion;
		}

		private void RestoreWindowSize()
		{
			var window = System.Windows.Application.Current.MainWindow;
			if (window != null)
			{
				if (Settings.Current.Orientation == OrientationType.Horizontal)
				{
					window.Width = Settings.Current.HorizontalSize.X;
					window.Height = Settings.Current.HorizontalSize.Y;
				}
				else
				{
					window.Width = Settings.Current.VerticalSize.X;
					window.Height = Settings.Current.VerticalSize.Y;
				}
			}
		}

	}
}
