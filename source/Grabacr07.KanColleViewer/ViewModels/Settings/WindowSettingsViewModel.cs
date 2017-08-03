﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Livet;
using MetroTrilithon.Linq;
using MetroTrilithon.Mvvm;
using System.Windows;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class WindowSettingsViewModel : ViewModel
	{
		private KanColleWindowSettings settings;

		public IReadOnlyCollection<DisplayViewModel<ExitConfirmationType>> ExitConfirmationTypes { get; }
		public IReadOnlyCollection<DisplayViewModel<ExitConfirmationType>> RefreshConfirmationTypes { get; }


		public IReadOnlyCollection<DisplayViewModel<string>> TaskbarProgressFeatures { get; }

		#region IsSplit 変更通知プロパティ

		private bool _IsSplit;

		public bool IsSplit
		{
			get { return this._IsSplit; }
			set
			{
				if (this._IsSplit != value)
				{
					this._IsSplit = value;
					this.RaisePropertyChanged();
				}
			}
		}

        #endregion

        #region AlwaysTopView 変更通知プロパティ

        private bool _AlwaysTopView;

		public bool AlwaysTopView
        {
			get { return this._AlwaysTopView; }
			set
			{
				if (this._AlwaysTopView != value)
				{
					this._AlwaysTopView = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Dock 変更通知プロパティ

		private Dock _Dock;

		public Dock Dock
		{
			get { return this._Dock; }
			set
			{
				if (this._Dock != value)
				{
					this._Dock = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.DockLeft));
					this.RaisePropertyChanged(nameof(this.DockTop));
					this.RaisePropertyChanged(nameof(this.DockRight));
					this.RaisePropertyChanged(nameof(this.DockBottom));
				}
			}
		}

		public bool DockLeft => this.Dock == Dock.Left;
		public bool DockTop => this.Dock == Dock.Top;
		public bool DockRight => this.Dock == Dock.Right;
		public bool DockBottom => this.Dock == Dock.Bottom;

		#endregion

		public WindowSettingsViewModel()
		{
			this.ExitConfirmationTypes = new List<DisplayViewModel<ExitConfirmationType>>
			{
				DisplayViewModel.Create(ExitConfirmationType.None,"확인하지않음"),
				DisplayViewModel.Create(ExitConfirmationType.InSortieOnly, "출격중에만 확인"),
				DisplayViewModel.Create(ExitConfirmationType.Always, "언제나 확인"),
			};
			this.RefreshConfirmationTypes = new List<DisplayViewModel<ExitConfirmationType>>
			{
				DisplayViewModel.Create(ExitConfirmationType.None,"확인하지않음"),
				DisplayViewModel.Create(ExitConfirmationType.InSortieOnly, "출격중에만 확인"),
				DisplayViewModel.Create(ExitConfirmationType.Always, "언제나 확인"),
			};
			this.TaskbarProgressFeatures = MetroTrilithon.Linq.EnumerableEx
				.Return(GeneralSettings.TaskbarProgressSource.ToDefaultDisplay("사용안함"))
				.Concat(TaskbarProgress.Features.ToDisplay(x => x.Id, x => x.DisplayName))
				.ToList();
		}

		public void Initialize()
		{
			this.settings = SettingsHost.Instance<KanColleWindowSettings>();
			this.settings?.IsSplit.Subscribe(x => this.IsSplit = x).AddTo(this);
            this.settings?.AlwaysTopView.Subscribe(x => this.AlwaysTopView = x).AddTo(this);
			this.settings?.Dock.Subscribe(x => this.Dock = x).AddTo(this);
		}

		public void SetDockSettings(Dock dock)
		{
			this.Dock = dock;
		}

		public void Apply()
		{
			if (this.settings != null)
			{
				this.settings.IsSplit.Value = this.IsSplit;
                this.settings.AlwaysTopView.Value = this.AlwaysTopView;
                this.settings.Dock.Value = this.Dock;
				try
				{
					if (WindowService.Current != null)
					{
						if (this.settings?.Dock == Dock.Right || this.settings?.Dock == Dock.Left)
						{
							WindowService.Current.Information.Vertical = Visibility.Collapsed;
							WindowService.Current.Information.Horizontal = Visibility.Visible;
							WindowService.Current.Information.Overview.Vertical = Visibility.Collapsed;
							WindowService.Current.Information.Overview.Horizontal = Visibility.Visible;
						}
						else
						{
							WindowService.Current.Information.Vertical = Visibility.Visible;
							WindowService.Current.Information.Horizontal = Visibility.Collapsed;
							WindowService.Current.Information.Overview.Vertical = Visibility.Visible;
							WindowService.Current.Information.Overview.Horizontal = Visibility.Collapsed;
						}
						WindowService.Current.UpdateDockPattern();
					}
				}
				catch { }
			}
		}

		public void Cancel()
		{
			if (this.settings != null)
			{
				this.IsSplit = this.settings.IsSplit;
                this.AlwaysTopView = this.settings.AlwaysTopView;
				this.Dock = this.settings.Dock;
			}
		}
	}
}
