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
using Livet;
using MetroRadiance;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;
using Settings = Grabacr07.KanColleViewer.Models.Settings;
using System.IO.Compression;
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
			//142에서 철거
			//using System.IO.Compression; 참조도 제거해야함
			#region temp code
			string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			if (File.Exists(Path.Combine(MainFolder, "AutoUpdater.exe")))
			{
				FileVersionInfo NowVersion = FileVersionInfo.GetVersionInfo(Path.Combine(MainFolder, "AutoUpdater.exe"));
				if (NowVersion.FileVersion == "1.0.0.0")
				{
					using (WebClient Client = new WebClient())
					{
						if (!Directory.Exists(Path.Combine(MainFolder, "tmp")))
							Directory.CreateDirectory(Path.Combine(MainFolder, "tmp"));
						Client.DownloadFile("https://github.com/FreyYa/KCVAutoUpdater/releases/download/1.0.1.0/AutoUpdater.zip", Path.Combine(MainFolder, "updater.zip"));
						ZipFile.ExtractToDirectory(Path.Combine(MainFolder, "updater.zip"), Path.Combine(MainFolder, "tmp"));
						File.Copy(Path.Combine(MainFolder, "tmp", "AutoUpdater.exe"), Path.Combine(MainFolder, "AutoUpdater.exe"), true);
						File.Copy(Path.Combine(MainFolder, "tmp", "AutoUpdater.exe.config"), Path.Combine(MainFolder, "AutoUpdater.exe.config"), true);
						File.Copy(Path.Combine(MainFolder, "tmp", "lib", "KCVKiller.dll"), Path.Combine(MainFolder, "lib", "KCVKiller.dll"), true);
						File.Delete(Path.Combine(MainFolder, "updater.zip"));
						Directory.Delete(Path.Combine(MainFolder, "tmp"), true);
					}
				}
			}
			#endregion

			this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception);

			DispatcherHelper.UIDispatcher = this.Dispatcher;
			ProductInfo = new ProductInfo();

			Settings.Load();
			PluginHost.Instance.Initialize();
			NotifierHost.Instance.Initialize(KanColleClient.Current);
			Helper.SetRegistryFeatureBrowserEmulation();
			Helper.SetMMCSSTask();

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

					if (File.Exists(Path.Combine(MainFolder, "AutoUpdater.exe")))
					{
						this.IsUpdate = true;
						Process MyProcess = new Process();
						MyProcess.StartInfo.FileName = "AutoUpdater.exe";
						MyProcess.StartInfo.WorkingDirectory = MainFolder;
						MyProcess.Start();
						MyProcess.Refresh();
					}
					else
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
			}
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
