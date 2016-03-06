﻿using System;
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

namespace Grabacr07.KanColleViewer
{
	public enum WindowServiceMode
	{
		/// <summary>
		/// 艦これが起動されていません。
		/// </summary>
		NotStarted,

		/// <summary>
		/// 艦これが起動されています。
		/// </summary>
		Started,

		/// <summary>
		/// 艦これが起動されており、艦隊が出撃中です。
		/// </summary>
		InSortie,
	}

	public class WindowService : NotificationObject, IDisposableHolder
	{
		public static WindowService Current { get; } = new WindowService();

		private WindowServiceMode currentMode = (WindowServiceMode)(-1); // 初回で setter 入るように
		private ObservableSynchronizedCollection<ViewModel> contents; 
		private InformationViewModel information;
		private KanColleWindowViewModel kanColleWindow;
		private InformationWindowViewModel informationWindow;
		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private KanColleWindowSettings settings;

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

							if(this.contents == null)
								this.contents = new ObservableSynchronizedCollection<ViewModel>();
							this.contents.Clear();
							this.contents.Add(startContent);

							this.MainWindow.Content = startContent;
							this.MainWindow.Contents = this.contents;
							this.MainWindow.StatusBar = startContent;
							StatusService.Current.Set(Resources.StatusBar_NotStarted);
							ThemeService.Current.ChangeAccent(Accent.Purple);
							break;
						case WindowServiceMode.Started:
							this.MainWindow.Content = this.Information;
							this.MainWindow.Contents = this.Informations;
							this.MainWindow.StatusBar = this.Information.SelectedItem;
							StatusService.Current.Set(Resources.StatusBar_Ready);
							ThemeService.Current.ChangeAccent(Accent.Blue);
							break;
						case WindowServiceMode.InSortie:
							ThemeService.Current.ChangeAccent(Accent.Orange);
							break;
					}

					this.RaisePropertyChanged();
				}
			}
		}

		/// <summary>
		/// 現在のメイン ウィンドウに提供されるデータを取得します。
		/// </summary>
		public MainWindowViewModelBase MainWindow { get; private set; }

		public InformationViewModel Information
		{
			get
			{
				if (this.information == null)
				{
					this.information = (InformationViewModel)this.Informations[0];
					this.information
						.Subscribe(nameof(InformationViewModel.SelectedItem), () => this.MainWindow.StatusBar = this.Information.SelectedItem)
						.AddTo(this);
				}
				return this.information;
			}
		}

		public ObservableSynchronizedCollection<ViewModel> Informations
		{
			get
			{
				if (this.settings == null)
					this.settings = SettingsHost.Instance<KanColleWindowSettings>();

				if (this.contents == null)
					this.contents = new ObservableSynchronizedCollection<ViewModel>();

				if (this.contents.Count > 0 && this.contents[0] is InformationViewModel)
					return this.contents;

				System.Windows.Application.Current.Dispatcher.Invoke(() =>
				{
					this.contents.Clear();
					var informationNum = this.settings?.IsDivide ? 2 : 1;
					for (var i = 0; i < informationNum; i++)
					{
						var infoViewModel = new InformationViewModel().AddTo(this);
						infoViewModel.IsPrimary = i == 0;
						this.contents.Add(infoViewModel);
					}
				});

				return this.contents;
			}
		}

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

		public void ClearZoomFactor()
		{
			this.kanColleWindow?.Messenger.Raise(new InteractionMessage { MessageKey = "WebBrowser.Zoom" });
		}

		public void SetLocationLeft()
		{
			this.kanColleWindow?.Messenger.Raise(new SetWindowLocationMessage { MessageKey = "Window.Location", Left = 0.0 });
		}


		public Window GetMainWindow()
		{
			if (this.MainWindow == this.kanColleWindow)
			{
				return new KanColleWindow { DataContext = this.kanColleWindow, };
			}
			if (this.MainWindow == this.informationWindow)
			{
				return new InformationWindow { DataContext = this.informationWindow, };
			}

			throw new InvalidOperationException();
		}


		private void UpdateMode()
		{
			this.Mode = KanColleClient.Current.IsStarted
				? KanColleClient.Current.IsInSortie ? WindowServiceMode.InSortie : WindowServiceMode.Started
				: WindowServiceMode.NotStarted;
		}

		#region disposable members

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

		public void Dispose()
		{
			this.compositeDisposable.Dispose();
		}

		#endregion
	}
}
