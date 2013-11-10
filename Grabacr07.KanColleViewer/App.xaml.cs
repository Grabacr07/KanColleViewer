using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Model;
using Grabacr07.KanColleWrapper;
using Settings = Grabacr07.KanColleViewer.Properties.Settings;

namespace Grabacr07.KanColleViewer
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App
	{
		public new App Current
		{
			get { return (App)Application.Current; }
		}


		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			KanColleClient.Current.Proxy.Startup(Settings.Default.LocalProxyPort);
			Model.Settings.Load();

			if (Toast.IsSupported)
			{
				Toast.TryInstallShortcut();
			}
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			KanColleClient.Current.Proxy.Shutdown();
			Model.Settings.Current.Save();
		}
	}
}
