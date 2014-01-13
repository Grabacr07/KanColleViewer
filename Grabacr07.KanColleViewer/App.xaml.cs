using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;
using Settings = Grabacr07.KanColleViewer.Model.Settings;

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

			this.DispatcherUnhandledException += (sender, args) => ReportException(sender, args.Exception);

			DispatcherHelper.UIDispatcher = this.Dispatcher;
			KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
			Settings.Load();

			ProductInfo = new ProductInfo();

			var proxy = KanColleClient.Current.Proxy;
			proxy.UpstreamProxyHost = Settings.Current.ProxyHost;
			proxy.UpstreamProxyPort = Settings.Current.ProxyPort;
			proxy.UseProxyOnConnect = Settings.Current.EnableProxy;
			proxy.UseProxyOnSSLConnect = Settings.Current.EnableSSLProxy;

			if (Toast.IsSupported)
			{
				Toast.TryInstallShortcut();
			}
			else
			{
				NotifyIconWrapper.Initialize();
			}

			ThemeService.Current.Initialize(this);

			ViewModelRoot = new MainWindowViewModel();
			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
			this.MainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			if (!Toast.IsSupported)
			{
				NotifyIconWrapper.Dispose();
			}
			KanColleClient.Current.Proxy.Shutdown();
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
	}
}
