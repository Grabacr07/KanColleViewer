using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleViewer.ViewModels.Settings;
using Grabacr07.KanColleViewer.Views;
using Grabacr07.KanColleViewer.Views.Controls;
using Livet.Messaging;
using MetroTrilithon.Mvvm;
using MetroTrilithon.UI.Controls;

namespace Grabacr07.KanColleViewer.ViewModels
{
	/// <summary>
	/// 艦これ (Flash 画面) を表示するメイン ウィンドウのためのデータを提供します。
	/// </summary>
	public class KanColleWindowViewModel : MainWindowViewModelBase
	{
		// 分割されたやつ
		private InformationWindowViewModel splitWindow;
		private readonly TaskbarProgress taskbarProgress;

		public NavigatorViewModel Navigator { get; }

		public BrowserZoomFactor ZoomFactor { get; }

		public VolumeViewModel Volume { get; }

		public KanColleWindowSettings Settings { get; }

		#region ContentVisibility 変更通知プロパティ

		private Visibility _ContentVisibility;

		public Visibility ContentVisibility
		{
			get { return this._ContentVisibility; }
			set
			{
				if (this._ContentVisibility != value)
				{
					this._ContentVisibility = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BrowserDock 変更通知プロパティ

		private Dock _BrowserDock;

		public Dock BrowserDock
		{
			get { return this._BrowserDock; }
			set
			{
				if (this._BrowserDock != value)
				{
					this._BrowserDock = value;
					this.BrowserDock2 = value == Dock.Bottom ? Dock.Bottom : Dock.Top;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region BrowserDock2 変更通知プロパティ

		private Dock _BrowserDock2 = Dock.Top;

		public Dock BrowserDock2
		{
			get { return this._BrowserDock2; }
			set
			{
				if (this._BrowserDock2 != value)
				{
					this._BrowserDock2 = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region ToolAreaMaxWidth 変更通知プロパティ

		private double _ToolAreaMaxWidth;

		public double ToolAreaMaxWidth
		{
			get { return this._ToolAreaMaxWidth; }
			set
			{
				if (!this._ToolAreaMaxWidth.Equals(value))
				{
					this._ToolAreaMaxWidth = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion
		
		#region InfoViewGridColumnNumber変更通知プロパティ
		private int _InfoViewGridColumnNumber = 1;

		public int InfoViewGridColumnNumber
		{
			get
			{ return this._InfoViewGridColumnNumber; }
			set
			{ 
				if (this._InfoViewGridColumnNumber == value)
					return;
				this._InfoViewGridColumnNumber = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		public KanColleWindowViewModel(bool isMainWindow) : base(isMainWindow)
		{
			this.Settings = SettingsHost.Instance<KanColleWindowSettings>();
			this.Settings.Dock.Subscribe(x => this.BrowserDock = x.Reverse()).AddTo(this);
			this.Settings.Dock.Subscribe(x => this.ToolAreaMaxWidth = this.GetToolAreaWidth(x)).AddTo(this);
			this.Settings.Dock.Subscribe(this.ChangeInfoViewGridColumnNumber).AddTo(this);
			this.Settings.IsSplit.Subscribe(this.SplitWindow).AddTo(this);
			this.Settings.IsDivide.Subscribe(this.DivideWindow).AddTo(this);

			this.Navigator = new NavigatorViewModel().AddTo(this);
			this.Volume = new VolumeViewModel().AddTo(this);

			this.ZoomFactor = new BrowserZoomFactor { Current = GeneralSettings.BrowserZoomFactor };
			this.ZoomFactor
				.Subscribe(nameof(this.ZoomFactor.Current), () => GeneralSettings.BrowserZoomFactor.Value = this.ZoomFactor.Current)
				.AddTo(this);

			GeneralSettings.BrowserZoomFactor.Subscribe(x => this.ZoomFactor.Current = x).AddTo(this);

			this.taskbarProgress = new TaskbarProgress().AddTo(this);
			this.taskbarProgress
				.Subscribe(nameof(TaskbarProgress.Updated), () => this.UpdateTaskbar())
				.AddTo(this);
		}


		protected override void InitializeCore()
		{
			SettingsViewModel.Instance.Initialize();
			SettingsViewModel.Instance.Navigator = this.Navigator;

			if (this.Settings.IsSplit)
			{
				// ウィンドウ表示時点で既に分割設定されていた場合、このタイミングで分割ウィンドウも一緒に表示
				this.Transition(this.splitWindow, typeof(InformationWindow), TransitionMode.NewOrActive, false);
			}

			this.UpdateTaskbar();
		}


		public void TakeScreenshot()
		{
			var path = Helper.CreateScreenshotFilePath();
			var message = new ScreenshotMessage("Screenshot.Save") { Path = path, };

			this.Messenger.Raise(message);

			var notify = message.Response.IsSuccess
				? Resources.Screenshot_Saved + Path.GetFileName(path)
				: Resources.Screenshot_Failed + message.Response.Exception.Message;
			StatusService.Current.Notify(notify);
		}


		/// <summary>
		/// <see cref="Information"/> をホストする別ウィンドウを表示し、アタッチされたメイン ウィンドウから <see cref="MainWindowViewModelBase.Content"/> を非表示にします。
		/// </summary>
		public void SplitWindow()
		{
			if (this.Settings.IsSplit && this.splitWindow != null)
			{
				this.splitWindow.Activate();
			}
			else
			{
				this.splitWindow = new InformationWindowViewModel(this);
				this.splitWindow.Closed += this.HandleSplitWindowClosed;

				if (this.IsInitialized) this.Transition(this.splitWindow, typeof(InformationWindow), TransitionMode.NewOrActive, false);

				this.ContentVisibility = Visibility.Collapsed;
				this.Settings.IsSplit.Value = true;
				this.UpdateTaskbar();
			}
		}

		/// <summary>
		/// <see cref="Information"/> をホストしている別ウィンドウを閉じ、アタッチされたメイン ウィンドウで <see cref="MainWindowViewModelBase.Content"/> を表示します。
		/// </summary>
		public void MergeWindow()
		{
			if (this.splitWindow != null)
			{
				this.splitWindow.Closed -= this.HandleSplitWindowClosed;
				this.splitWindow.Close();
				this.splitWindow = null;

				if (!this.IsClosed)
				{
					// このウィンドウが既に閉じられた後だったとき (= アプリ終了するとき) は
					// Split 設定を上書きしないようにする
					this.ContentVisibility = Visibility.Visible;
					this.Settings.IsSplit.Value = false;
				}

				this.UpdateTaskbar();
			}
		}


		private void SplitWindow(bool split)
		{
			if (split)
			{
				this.SplitWindow();
			}
			else
			{
				this.MergeWindow();
			}
		}

		private void DivideWindow(bool divide)
		{
			if (this.Contents == null || this.Contents[0] is StartContentViewModel)
				return;
			
			if (this.Settings.IsSplit)
				return;

			var infoWindowNum = divide ? 2 : 1;

			if (this.Contents.Count == infoWindowNum)
				return;

			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				this.Contents.Clear();
				this.AddInformationViewModelToContents(infoWindowNum);
				this.Content = this.Contents[0];
			});

			this.ChangeInfoViewGridColumnNumber();
		}

		/// <summary>
		/// Add setted number of InformationViewModel to Contents
		/// </summary>
		/// <param name="num">Describe how many ViewModel to be add</param>
		private void AddInformationViewModelToContents(int num = 1)
		{
			for (var i = 0; i < num; i++)
			{
				var infoViewModel = new InformationViewModel().AddTo(this);
				infoViewModel.IsPrimary = i == 0;	// 追加するInformationViewModelがPrimaryであるか否かを設定
				this.Contents.Add(infoViewModel);
			}
		}

		private void ChangeInfoViewGridColumnNumber()
		{
			this.ChangeInfoViewGridColumnNumber(this.Settings.Dock);
		}

		/// <summary>
		/// Change the Grid's Column number with Dock's status
		/// </summary>
		private void ChangeInfoViewGridColumnNumber(Dock currentDock)
		{
			// DockがLeft/Rightの際には縦に並べたいので Columns を 1 に設定する
			if (currentDock == Dock.Left || currentDock == Dock.Right)
				this.InfoViewGridColumnNumber = 1;

			// DockがTop/Bottomの際には横に並べたいので Columns を 2 に設定する
			if (currentDock == Dock.Top || currentDock == Dock.Bottom)
				this.InfoViewGridColumnNumber = 2;
		}

		private void HandleSplitWindowClosed(object sender, EventArgs eventArgs)
		{
			this.MergeWindow();
		}

		private double GetToolAreaWidth(Dock d)
		{
			switch (d)
			{
				case Dock.Left:
				case Dock.Right:
					return KanColleHost.KanColleSize.Width;
				default:
					return double.PositiveInfinity;
			}
		}

		private void UpdateTaskbar()
		{
			// 分割ウィンドウがいなかったら、自身のタスク バーを設定する
			// 分割ウィンドウがいる場合はそっちに任せる

			if (this.splitWindow == null)
			{
				this.UpdateTaskbar(this.taskbarProgress.State, this.taskbarProgress.Value);
			}
			else
			{
				this.UpdateTaskbar(TaskbarItemProgressState.None, .0);
			}
		}
	}
}
