using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.Messaging;
using MetroRadiance.UI;
using MetroTrilithon.Lifetime;
using MetroTrilithon.Mvvm;
using System.Windows.Input;
using System.Windows.Controls;

namespace Grabacr07.KanColleViewer
{
	public enum WindowServiceMode
	{
		/// <summary>
		/// 칸코레가 기동되지 않은 경우
		/// </summary>
		NotStarted,

		/// <summary>
		/// 칸코레가 기동된 경우
		/// </summary>
		Started,

		/// <summary>
		/// 칸코레가 기동되었고, 함대가 출격중인 경우
		/// </summary>
		InSortie,
	}

	public class WindowService : NotificationObject, IDisposableHolder
	{
		public static WindowService Current { get; } = new WindowService();

		private WindowServiceMode currentMode = (WindowServiceMode)(-1); // 처음에 Setter에 진입하기 위해

		private KanColleWindowViewModel kanColleWindow; // 메인 윈도우
		private InformationViewModel information; // 정보 영역
		private InformationWindowViewModel informationWindow; // 시작 전 정보 영역

		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();

		private WindowService() { }

		public void Initialize()
		{
			if (GeneralSettings.IsProxyMode)
			{
				// プロキシ モード (艦これのウィンドウを表示しないやつ)
				// KanColleWindow は作らず、InformationWindow を MainWindow として運用する
				this.informationWindow = new InformationWindowViewModel(true);
				this.MainWindow = this.informationWindow;
			}
			else
			{
				// 通常モード ((艦これ + 情報ウィンドウ) or その分割)
				this.kanColleWindow = new KanColleWindowViewModel(true);
				this.MainWindow = this.kanColleWindow;
			}

			KanColleClient.Current.Subscribe(nameof(KanColleClient.IsStarted), this.UpdateMode).AddTo(this);
			KanColleClient.Current.Subscribe(nameof(KanColleClient.IsInSortie), this.UpdateMode).AddTo(this);
		}

		// 정보 영역 위치 패턴을 갱신
		public void UpdateDockPattern()
		{
			// 기동 전 혹은 메인 윈도우가 없다면 무시
			if (this.currentMode == WindowServiceMode.NotStarted) return;
			if (this.kanColleWindow == null) return;

			KanColleWindowSettings settings = SettingsHost.Instance<KanColleWindowSettings>();
			if (settings.AlwaysTopView) // 항상 탑 뷰인 경우
			{
				this.kanColleWindow.TopView = Visibility.Visible;
				this.kanColleWindow.BottomView = Visibility.Collapsed;
			}
			else if (!settings.IsSplit) // 분할 모드가 아닌 경우
			{
				if (settings?.Dock == Dock.Right || settings?.Dock == Dock.Left)
				{
					this.kanColleWindow.TopView = Visibility.Visible;
					this.kanColleWindow.BottomView = Visibility.Collapsed;
				}
				else
				{
					this.kanColleWindow.TopView = Visibility.Collapsed;
					this.kanColleWindow.BottomView = Visibility.Visible;
				}
			}
			else
			{
				this.kanColleWindow.TopView = Visibility.Collapsed;
				this.kanColleWindow.BottomView = Visibility.Visible;
			}
		}

		/// <summary>
		/// 현재 기동 상태
		/// </summary>
		public WindowServiceMode Mode
		{
			get { return this.currentMode; }
			set
			{
				if (this.currentMode != value)
				{
					this.currentMode = value;
					switch (value)
					{
						case WindowServiceMode.NotStarted:
							var startContent = new StartContentViewModel(this.kanColleWindow?.Navigator);
							this.MainWindow.Content = startContent;
							this.MainWindow.StatusBar = startContent;
							StatusService.Current.Set(Resources.StatusBar_NotStarted);
							ThemeService.Current.ChangeAccent(Accent.Purple);
							break;
						case WindowServiceMode.Started:
							this.MainWindow.Content = this.Information;
							this.MainWindow.StatusBar = this.Information.SelectedItem;
							StatusService.Current.Set(Resources.StatusBar_Ready);
							ThemeService.Current.ChangeTheme(Theme.Dark);
							ThemeService.Current.ChangeAccent(Accent.Blue);

							this.UpdateDockPattern();
							break;
					}

					this.RaisePropertyChanged();
				}
			}
		}

		/// <summary>
		/// 현재 메인 윈도우에 표시되는 뷰 모델
		/// </summary>
		public MainWindowViewModelBase MainWindow { get; private set; }

		/// <summary>
		/// Information 뷰 모델
		/// </summary>
		public InformationViewModel Information
		{
			get
			{
				if (this.information == null)
				{
					this.information = new InformationViewModel().AddTo(this);
					this.information
						.Subscribe(
							nameof(InformationViewModel.SelectedItem),
							() => this.MainWindow.StatusBar = this.Information.SelectedItem
						)
						.AddTo(this);
				}
				return this.information;
			}
		}

		/// <summary>
		/// 게임 페이지 확대 100%로 초기화
		/// </summary>
		public void ClearZoomFactor()
		{
			this.kanColleWindow?.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser.Zoom" });
		}

		/// <summary>
		/// 게임 페이지 새로고침
		/// </summary>
		public void RefreshWindow()
		{
			this.kanColleWindow?.RefreshNavigator.Execute(null);
		}
		public ICommand RefreshRemote() => this.kanColleWindow?.RefreshNavigator;

		/// <summary>
		/// 화면의 제일 왼쪽으로 이동
		/// </summary>
		public void SetLocationLeft()
		{
			this.kanColleWindow?.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window.Location", Left = 0.0 });
		}

		/// <summary>
		/// 표시중인 내용 윈도우를 반환
		/// </summary>
		/// <returns></returns>
		public Window GetMainWindow()
		{
			// 표시중인 뷰에 따라 반환
			if (this.MainWindow == this.kanColleWindow)
				return new KanColleWindow { DataContext = this.kanColleWindow, };

			else if (this.MainWindow == this.informationWindow)
				return new InformationWindow { DataContext = this.informationWindow, };

			throw new InvalidOperationException();
		}

		/// <summary>
		/// 이 객체의 Mode 를 알아서 설정
		/// </summary>
		private void UpdateMode()
		{
			this.Mode = KanColleClient.Current.IsStarted
				? KanColleClient.Current.IsInSortie
					? WindowServiceMode.InSortie : WindowServiceMode.Started
				: WindowServiceMode.NotStarted;
		}

		#region Disposable 멤버

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;
		public void Dispose()
		{
			this.compositeDisposable.Dispose();
		}

		#endregion
	}
}
