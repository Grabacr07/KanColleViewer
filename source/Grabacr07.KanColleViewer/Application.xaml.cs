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
using Grabacr07.KanColleWrapper.Models;
using Grabacr07.KanColleWrapper;
using Livet;
using MetroRadiance.UI;
using MetroTrilithon.Lifetime;
using KanColleSettings = Grabacr07.KanColleViewer.Models.Settings.KanColleSettings;
using AppSettings = Grabacr07.KanColleViewer.Properties.Settings;

namespace Grabacr07.KanColleViewer
{
	/// <summary>
	/// 어플리케이션의 상태를 가리키는 식별자를 정의합니다
	/// </summary>
	public enum ApplicationState
	{
		/// <summary>
		/// 어플리케이션이 기동중입니다.
		/// </summary>
		Startup,

		/// <summary>
		/// 어플리케이션의 기동 준비가 완료되어서 실행중입니다.
		/// </summary>
		Running,

		/// <summary>
		/// 어플리케이션이 종료되었지만, 아직 종료 처리중입니다.
		/// </summary>
		Terminate,
	}

	sealed partial class Application : INotifyPropertyChanged, IDisposableHolder
	{
		private static string CurrentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		private GCWorker bgGCWorker { get; set; } // 백그라운드 메모리 정리 워커

		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private event PropertyChangedEventHandler propertyChangedInternal;

		static Application()
		{
			AppDomain.CurrentDomain.UnhandledException += (sender, args)
				=> ReportException(sender, args.ExceptionObject as Exception);
		}

		/// <summary>
		/// 현재 <see cref="AppDomain"/>의 <see cref="Application"/> 객체를 반환합니다.
		/// </summary>
		public static Application Instance => Current as Application;

		/// <summary>
		/// 어플리케이션의 현재 상태를 가리키는 식별자를 반환합니다.
		/// </summary>
		public ApplicationState State { get; private set; }

		// 시작할 때의 이벤트
		protected override void OnStartup(StartupEventArgs e)
		{
			this.ChangeState(ApplicationState.Startup);
			bgGCWorker = new GCWorker();

			// 개발중에 다중 실행이 감지되어서 실행되지 않으면 불편하니 디버그 때에는 예외로 처리
#if !DEBUG
			var appInstance = new MetroTrilithon.Desktop.ApplicationInstance().AddTo(this);
			if (appInstance.IsFirst)
#endif
			{
				// 미처리 예외
				this.DispatcherUnhandledException += (sender, args) =>
				{
					ReportException(sender, args.Exception);
					args.Handled = true;
				};

				DispatcherHelper.UIDispatcher = this.Dispatcher;

				SettingsHost.Load(); // 설정들 로드
				this.compositeDisposable.Add(SettingsHost.Save); // Dispose될 때 설정을 저장

				try
				{
					// 언어 변경
					GeneralSettings.Culture
						.Subscribe(x => ResourceService.Current.ChangeCulture(x))
						.AddTo(this);
				}
				catch
				{
					// 무언가 오류가 발생...
					try
					{
						File.Delete(Providers.LocalFilePath);
						File.Delete(Providers.ViewerDirectoryPath);
					}
					catch { }

					MessageBox.Show(
						"어플리케이션 기동중 문제가 발생하여 일부 설정파일을 초기화합니다.\n"
							+ "프로그램이 종료되며, 재시작 해주시기 바랍니다.",
						"제독업무도 바빠!",
						MessageBoxButton.OK,
						MessageBoxImage.Warning
					);
					this.Shutdown();
					return;
				}

				// 기존 설정 불러오기..?
				if (KanColleViewer.Properties.Settings.Default.UpdateSettings)
				{
					KanColleViewer.Properties.Settings.Default.Upgrade();
					KanColleViewer.Properties.Settings.Default.UpdateSettings = false;
					KanColleViewer.Properties.Settings.Default.Save();
				}

				// 설정들 초기화
				KanColleClient.Current.Settings = new KanColleSettings();

				// 테마 변경
				ThemeService.Current.Register(this, Theme.Dark, Accent.Purple);

				// 각종 서비스 초기화
				PluginService.Current.AddTo(this).Initialize();
				WindowService.Current.AddTo(this).Initialize();
				NotifyService.Current.AddTo(this).Initialize();

				// WebBrowser 컨트롤 IE 버전 레지스트리 패치, MMCSS 설정
				Helper.SetRegistryFeatureBrowserEmulation();
				if (GeneralSettings.MMCSSEnabled) Helper.SetMMCSSTask();

				// 번역 여부
				KanColleClient.Current.Translations.EnableTranslations = KanColleSettings.EnableTranslations;
				KanColleClient.Current.Translations.EnableAddUntranslated = KanColleSettings.EnableAddUntranslated;

				// 설정된 경우, 업데이트 통지 및 다운로드
				if (KanColleClient.Current.Updater.LoadVersion(AppSettings.Default.KCVUpdateUrl.AbsoluteUri))
				{
					// 업데이트 알림이 설정되어있고 상위 버전이 있을 경우
					if (KanColleSettings.EnableUpdateNotification && KanColleClient.Current.Updater.IsOnlineVersionGreater(TranslationType.App, ProductInfo.Version.ToString()))
					{
						// 자동 업데이트 프로그램이 존재하는 경우
						if (File.Exists(Path.Combine(CurrentDirectory, "AutoUpdater.exe")))
						{
							// 자동 업데이트 프로그램을 실행
							Process MyProcess = new Process();
							MyProcess.StartInfo.FileName = "AutoUpdater.exe";
							MyProcess.StartInfo.WorkingDirectory = CurrentDirectory;
							MyProcess.Start();
							MyProcess.Refresh();
							this.Shutdown();
						}
						else
						{
							// 자동 업데이트 프로그램이 존재하지 않는 경우 알림만 표시
							var notification = Notification.Create(
								"",
								"제독업무도 바빠! 업데이트",
								"어플리케이션의 새로운 버전이 릴리즈 되었습니다!"
									+ "블로그 등의 배포처를 확인해주시기 바랍니다.",
								() => WindowService.Current.MainWindow.Activate()
							);
							NotifyService.Current.Notify(notification);
						}
					}

					// 번역 파일 업데이트가 설정된 경우
					if (KanColleSettings.EnableUpdateTransOnStart)
					{
						// 번역 파일들 업데이트
						if (KanColleClient.Current.Updater.UpdateTranslations(AppSettings.Default.XMLTransUrl.AbsoluteUri, KanColleClient.Current.Translations) > 0)
						{
							var notification = Notification.Create(
								"",
								"제독업무도 바빠! 업데이트",
								"번역 파일의 업데이트가 완료되었습니다.",
								() => WindowService.Current.MainWindow.Activate()
							);
							NotifyService.Current.Notify(notification);
						}
					}
				}

				// BootstrapProxy()에서 Views.Settings.ProxyBootstrapper.Show()가 호출되기 전에
				// Application.MainWindow를 설정해둔다. (매우 중요함)
				// 나중에 설정하는 경우, Views.Settings.ProxyBootstrapper가 닫히면 프로그램도 같이 닫힘.
				this.MainWindow = WindowService.Current.GetMainWindow();

				// 로컬 프록시를 시작한다.
				if (BootstrapProxy())
				{
					// 종료될 때 로컬 프록시를 종료한다.
					this.compositeDisposable.Add(ProxyBootstrapper.Shutdown);
					this.MainWindow.Show();

					// 브라우저 관리 클래스
					var navigator = (WindowService.Current.MainWindow as KanColleWindowViewModel)?.Navigator;
					if (navigator != null)
					{
						navigator.Source = KanColleViewer.Properties.Settings.Default.KanColleUrl;
						navigator.Navigate();
					}

#if !DEBUG
					// 디버그가 아닌 경우 다중 실행과 명령줄을 처리
					appInstance.CommandLineArgsReceived += (sender, args) =>
					{
						// 다중 실행을 감지한 경우, 메인 윈도우를 표시
						this.Dispatcher.Invoke(() => WindowService.Current.MainWindow.Activate());
						this.ProcessCommandLineParameter(args.CommandLineArgs);
					};
#endif

					base.OnStartup(e);
					this.ChangeState(ApplicationState.Running);
					// 실행중임을 처리
				}
				else
				{
					// 로컬 프록시 시작에 실패한 경우 그대로 종료
					this.ChangeState(ApplicationState.Terminate);
					this.Shutdown();
				}
			}
#if !DEBUG
			else
			{
				// 다중 실행인 경우 명령줄을 전달하고 종료
				appInstance.SendCommandLineArgs(e.Args);
				this.ChangeState(ApplicationState.Terminate);
				this.Shutdown();
			}
#endif
		}

		// 종료될 때의 이벤트 (취소 가능)
		protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
		{
			// 프로그램 종료 확인 설정
			var confirmation =
				GeneralSettings.ExitConfirmationType == ExitConfirmationType.Always
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

		// 종료될 때의 이벤트 (취소 불가능)
		protected override void OnExit(ExitEventArgs e)
		{
			// 전과 저장
			ViewModels.Contents.AdmiralViewModel.Record?.Save();

			this.ChangeState(ApplicationState.Terminate);
			base.OnExit(e);

			this.compositeDisposable.Dispose();
			bgGCWorker.Dispose();
		}

		/// <summary>
		/// <see cref="State"/> 프로퍼티를 변경하고, <see cref="INotifyPropertyChanged.PropertyChanged"/> 이벤트를 발생시킵니다.
		/// </summary>
		/// <param name="value">어플리케이션 기동 상태</param>
		private void ChangeState(ApplicationState value)
		{
			if (this.State == value) return;

			this.State = value;
			this.RaisePropertyChanged(nameof(this.State));
		}

		/// <summary>
		/// 명령줄 인수를 가지고 다중 실행된 경우에 처리합니다.
		/// </summary>
		/// <param name="args"></param>
		private void ProcessCommandLineParameter(string[] args)
		{
			// 지금은 할 일이 없다.
			Debug.WriteLine("다중 실행 감지: " + args.ToString(" "));
		}

		/// <summary>
		/// <see cref="ProxyBootstrapper"/> 를 사용하여, <see cref="KanColleProxy"/> 의 기동을 시도합니다.
		/// 필요에 따라 유저에게 작업을 요구하는 다이얼로그를 표시합니다.
		/// </summary>
		/// <returns><see cref="KanColleProxy"/> 의 기동에 성공했을 때에는 True, 그 외에는 False를 반환합니다.</returns>
		private static bool BootstrapProxy()
		{
			var bootstrapper = new ProxyBootstrapper();
			bootstrapper.Try();

			if (bootstrapper.Result == ProxyBootstrapResult.Success)
				return true;

			var vmodel = new ProxyBootstrapperViewModel(bootstrapper) { Title = ProductInfo.Title, };
			var window = new Views.Settings.ProxyBootstrapper { DataContext = vmodel, };
			window.ShowDialog();

			return vmodel.DialogResult;
		}

		/// <summary>
		/// 처리되지 않은 예외를 처리합니다.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="exception"></param>
		private static void ReportException(object sender, Exception exception)
		{
			#region const

			const string MessageFormat = @"
===========================================================
ERROR, date = {0}, sender = {1},
{2}
";
			const string Path = "error.log";

			#endregion

			try
			{
				var message = string.Format(MessageFormat, DateTimeOffset.Now, sender, exception);
				Debug.WriteLine(message);
				File.AppendAllText(Path, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			// 일단 종료시키는 수 밖에 없다.
			// 복구시킬 수 있다면 좋겠지만, 모릅니다.
			Current.Shutdown();
		}

		#region INotifyPropertyChanged 멤버

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { this.propertyChangedInternal += value; }
			remove { this.propertyChangedInternal -= value; }
		}

		private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
		{
			this.propertyChangedInternal?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion

		#region IDisposable 멤버

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

		void IDisposable.Dispose()
		{
			this.compositeDisposable.Dispose();
		}

		#endregion
	}
}
