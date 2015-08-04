using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroRadiance;
using MetroTrilithon.Lifetime;

namespace Grabacr07.KanColleViewer
{
	/// <summary>
	/// アプリケーションの状態を示す識別子を定義します。
	/// </summary>
	public enum ApplicationState
	{
		/// <summary>
		/// アプリケーションは起動中です。
		/// </summary>
		Startup,

		/// <summary>
		/// アプリケーションは起動準備が完了し、実行中です。
		/// </summary>
		Running,

		/// <summary>
		/// アプリケーションは終了したか、または終了処理中です。
		/// </summary>
		Terminate,
	}

	sealed partial class Application : INotifyPropertyChanged, IDisposableHolder
	{
		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args) => ReportException(sender, args.ExceptionObject as Exception);
		}

		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private event PropertyChangedEventHandler PropertyChangedInternal;

		/// <summary>
		/// 現在の <see cref="AppDomain"/> の <see cref="Application"/> オブジェクトを取得します。
		/// </summary>
		public static Application Instance => Current as Application;

		/// <summary>
		/// アプリケーションの現在の状態を示す識別子を取得します。
		/// </summary>
		public ApplicationState State { get; private set; }


		protected override void OnStartup(StartupEventArgs e)
		{
			this.ChangeState(ApplicationState.Startup);

			// 開発中に多重起動検知ついてると起動できなくて鬱陶しいので
			// デバッグ時は外すんじゃもん
#if !DEBUG
			var appInstance = new MetroTrilithon.Desktop.ApplicationInstance().AddTo(this);
			if (appInstance.IsFirst)
#endif
			{
				this.DispatcherUnhandledException += (sender, args) =>
				{
					ReportException(sender, args.Exception);
					args.Handled = true;
				};

				DispatcherHelper.UIDispatcher = this.Dispatcher;

				SettingsHost.Load();
				this.compositeDisposable.Add(SettingsHost.Save);

				GeneralSettings.Culture.Subscribe(x => ResourceService.Current.ChangeCulture(x)).AddTo(this);
				KanColleClient.Current.Settings = new KanColleSettings();

				ThemeService.Current.Initialize(this, Theme.Dark, Accent.Purple);
				WindowService.Current.AddTo(this).Initialize();
				PluginService.Current.AddTo(this).Initialize();
				NotifyService.Current.AddTo(this).Initialize();

				Helper.SetRegistryFeatureBrowserEmulation();
				Helper.SetMMCSSTask();

				// BootstrapProxy() で Views.Settings.ProxyBootstrapper.Show() が呼ばれるより前に
				// Application.MainWindow を設定しておく。これ大事
				this.MainWindow = WindowService.Current.GetMainWindow();

				if (BootstrapProxy())
				{
					this.compositeDisposable.Add(ProxyBootstrapper.Shutdown);
					this.MainWindow.Show();
#if !DEBUG
					appInstance.CommandLineArgsReceived += (sender, args) =>
					{
						// 多重起動を検知したら、メイン ウィンドウを最前面に出す
						this.Dispatcher.Invoke(() => WindowService.Current.MainWindow.Activate());
						this.ProcessCommandLineParameter(args.CommandLineArgs);
					};
#endif
					base.OnStartup(e);
					this.ChangeState(ApplicationState.Running);
				}
				else
				{
					this.ChangeState(ApplicationState.Terminate);
					this.Shutdown();
				}
			}
#if !DEBUG
			else
			{
				appInstance.SendCommandLineArgs(e.Args);
				this.ChangeState(ApplicationState.Terminate);
				this.Shutdown();
			}
#endif
		}


		protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
		{
			var confirmation = GeneralSettings.ExitConfirmationType == ExitConfirmationType.Always
							   || (GeneralSettings.ExitConfirmationType == ExitConfirmationType.InSortieOnly && KanColleClient.Current.IsInSortie);
			if (confirmation)
			{
				var vmodel = new DialogViewModel();
				var window = new ExitDialog
				{
					DataContext = vmodel,
					Owner = this.MainWindow,
				};
				window.ShowDialog();

				e.Cancel = !vmodel.DialogResult;
			}

			base.OnSessionEnding(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			this.ChangeState(ApplicationState.Terminate);
			base.OnExit(e);

			this.compositeDisposable.Dispose();
		}

		/// <summary>
		/// <see cref="State"/> プロパティを更新し、<see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを発生させます。
		/// </summary>
		/// <param name="value"></param>
		private void ChangeState(ApplicationState value)
		{
			if (this.State == value) return;

			this.State = value;
			this.RaisePropertyChanged(nameof(this.State));
		}

		private void ProcessCommandLineParameter(string[] args)
		{
			Debug.WriteLine("多重起動検知: " + args.ToString(" "));

			// コマンド ライン引数付きで多重起動されたときに何かできる
			// けど今やることがない
		}

		/// <summary>
		/// <see cref="ProxyBootstrapper"/> を使用し、<see cref="KanColleProxy"/> を起動することを試みます。
		/// 必要に応じて、ユーザーに操作を求めるダイアログを表示します。
		/// </summary>
		/// <returns><see cref="KanColleProxy"/> の起動に成功した場合は true、それ以外の場合は false。</returns>
		private static bool BootstrapProxy()
		{
			var bootstrapper = new ProxyBootstrapper();
			bootstrapper.Try();

			if (bootstrapper.Result == ProxyBootstrapResult.Success)
			{
				return true;
			}

			var vmodel = new ProxyBootstrapperViewModel(bootstrapper) { Title = ProductInfo.Title, };
			var window = new Views.Settings.ProxyBootstrapper { DataContext = vmodel, };
			window.ShowDialog();

			return vmodel.DialogResult;
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

			// ToDo: 例外ダイアログ

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

			// とりあえずもう終了させるしかないもじゃ
			// 救えるパターンがあるなら救いたいけど方法わからんもじゃ
			Current.Shutdown();
		}

		#region INotifyPropertyChanged members

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.PropertyChangedInternal += value; }
			remove { this.PropertyChangedInternal -= value; }
		}

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.PropertyChangedInternal?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region IDisposable members

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

		void IDisposable.Dispose()
		{
			this.compositeDisposable.Dispose();
		}

		#endregion
	}
}
