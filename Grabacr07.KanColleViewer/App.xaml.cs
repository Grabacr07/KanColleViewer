using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App
	{
		public static MainWindowViewModel ViewModelRoot { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			DispatcherHelper.UIDispatcher = this.Dispatcher;
			KanColleClient.Current.Proxy.Startup(AppSettings.Default.LocalProxyPort);
			Settings.Load();

			if (Toast.IsSupported)
			{
				Toast.TryInstallShortcut();
			}

			ThemeService.Current.Initialize(this);

			ViewModelRoot = new MainWindowViewModel();
			this.MainWindow = new MainWindow { DataContext = ViewModelRoot };
			this.MainWindow.Show();
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			KanColleClient.Current.Proxy.Shutdown();
			Settings.Current.Save();
		}
	}
}
