using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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

namespace Grabacr07.KanColleViewer
{
	public partial class App
	{
		public static ProductInfo ProductInfo { get; private set; }

		public static MainWindowViewModel ViewModelRoot { get; private set; }

		static App()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}


		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var appInstance = new ApplicationInstance();
			if (appInstance.IsFirst)
			{
				this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception);

				DispatcherHelper.UIDispatcher = this.Dispatcher;
				ProductInfo = new ProductInfo();

				Settings.Load();
				ResourceService.Current.ChangeCulture(Settings.Current.Culture);

				PluginHost.Instance.Initialize();
				NotifierHost.Instance.Initialize();
				Helper.SetRegistryFeatureBrowserEmulation();
				Helper.SetMMCSSTask();

				StartProxy();

				ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);

				ViewModelRoot = new MainWindowViewModel();
				this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
				this.MainWindow.Show();

				appInstance.CommandLineArgsReceived += (sender, args) =>
				{
					this.Dispatcher.Invoke(() => ViewModelRoot.Activate());
					this.ProcessCommandLineParameter(args.CommandLineArgs);
				};
			}
			else
			{
				appInstance.SendCommandLineArgs(e.Args);
				this.Shutdown();
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			KanColleClient.Current.Proxy.Shutdown();

			NotifierHost.Instance.Dispose();
			PluginHost.Instance.Dispose();

			Settings.Current.Save();
		}

		private static void StartProxy()
		{
			//既存設定がある場合、初回は0になってしまう。範囲外の場合デフォルトを設定。
			if (Settings.Current.LocalProxyPort < 1 || 65535 < Settings.Current.LocalProxyPort)
				Settings.Current.LocalProxyPort = AppSettings.Default.DefaultLocalProxyPort;
			var listeningPort = Settings.Current.IsEnableChangeLocalProxyPort
				? Settings.Current.LocalProxyPort
				: AppSettings.Default.DefaultLocalProxyPort;
			try
			{
				KanColleClient.Current.Proxy.Startup(listeningPort);
			}
			catch (SocketException ex)
			{
				if (ex.ErrorCode != 10048) throw;
				// 参照: Windows ソケットのエラー コード、値、および意味 https://support.microsoft.com/en-us/kb/819124/ja
				MessageBox.Show("既にポート " + listeningPort + " で通信を待ち受けているアプリケーションが存在するため、" +
								"待ち受けの開始に失敗しました。" + Environment.NewLine +
								"当該アプリケーションを終了するか、Settings＞通信設定 画面から待ち受けポートの変更を行って下さい。",
					ProductInfo.Title,
					MessageBoxButton.OK,
					MessageBoxImage.Error);
			}
			KanColleClient.Current.Proxy.UpstreamProxySettings = Settings.Current.ProxySettings;
		}

		private void ProcessCommandLineParameter(string[] args)
		{
			Debug.WriteLine("多重起動検知: " + args.ToString(" "));

			// コマンド ライン引数付きで多重起動されたときに何かできる
			// けど今やることがない
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
	}
}
