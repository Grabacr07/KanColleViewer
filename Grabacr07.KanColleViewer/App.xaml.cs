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
using System.Text;

namespace Grabacr07.KanColleViewer
{
	public partial class App
	{
		public static ProductInfo ProductInfo { get; private set; }
		public static MainWindowViewModel ViewModelRoot { get; private set; }
		private bool IsUpdate { get; set; }

		string MainFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

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

			//기본값을 설정.
			string portNumStr = string.Empty;
			int portNum = AppSettings.Default.LocalProxyPort;


			//port.txt이 존재하는 경우 파일에서 port번호를 읽는다
			if (File.Exists(Path.Combine(MainFolder, "Port.txt")))
			{
				var stream = new StreamReader(Path.Combine(MainFolder, "Port.txt"), Encoding.UTF8);

				portNumStr = stream.ReadToEnd();
				stream.Close();

				try
				{
					portNum = Convert.ToInt32(portNumStr);

					if (portNum != AppSettings.Default.LocalProxyPort)
					{
						try
						{
							KanColleClient.Current.Proxy.Startup(portNum);
						}
						catch(Exception ex)
						{
							KanColleClient.Current.CatchedErrorLogWriter.ReportException(ex.Source, ex);
							KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
						}
					}
					else
					{
						KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine("txt파일의 내용을 Int로 변경하는데 실패했습니다. Port번호는 기존 설정값을 유지합니다", ex);
					KanColleClient.Current.CatchedErrorLogWriter.ReportException(ex.Source, ex);

					WritePortFile(AppSettings.Default.LocalProxyPort);
				}

			}
			else//해당 파일이 없는 경우 파일을 기본값으로 작성한다.
			{
				WritePortFile(AppSettings.Default.LocalProxyPort);
				KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
			}


			KanColleClient.Current.Proxy.UpstreamProxySettings = Settings.Current.ProxySettings;

			ResourceService.Current.ChangeCulture(Settings.Current.Culture);
			ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);
			//Custom Settings
			//KanColleHost.Current.EnableResizing = Settings.Current.EnableResizing;
			KanColleClient.Current.Logger.EnableLogging = Settings.Current.EnableLogging;
			KanColleClient.Current.OracleOfCompass.EnableBattlePreview = Settings.Current.EnableBattlePreview;
			KanColleClient.Current.OracleOfCompass.initialialize();
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
				MessageBoxResult MB = MessageBox.Show("Internet Explorer용 Flash Player ActiveX가 설치되어있지않습니다. 지금 설치하시겠습니까?", "Adobe Flash ActiveX를 찾을 수 없습니다", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);
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
		private void WritePortFile(int portNum)
		{
			var stream = new StreamWriter(Path.Combine(MainFolder, "Port.txt"), false, Encoding.UTF8);
			stream.Write(Convert.ToString(portNum));
			stream.Flush();
			stream.Close();
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
