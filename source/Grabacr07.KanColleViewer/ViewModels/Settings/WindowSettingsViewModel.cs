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

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class WindowSettingsViewModel : ViewModel
	{
		private KanColleWindowSettings settings;

		public IReadOnlyCollection<DisplayViewModel<ExitConfirmationType>> ExitConfirmationTypes { get; }

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
		
		#region IsDivide変更通知プロパティ
		private bool _IsDivide;

		public bool IsDivide
		{
			get
			{ return this._IsDivide; }
			set
			{
				if (this._IsDivide != value)
				{
					this._IsDivide = value;
					this.RaisePropertyChanged();
				}
			}
		}
		#endregion

		public WindowSettingsViewModel()
		{
			this.ExitConfirmationTypes = new List<DisplayViewModel<ExitConfirmationType>>
			{
				DisplayViewModel.Create(ExitConfirmationType.None, "確認しない"),
				DisplayViewModel.Create(ExitConfirmationType.InSortieOnly, "出撃中のみ確認する"),
				DisplayViewModel.Create(ExitConfirmationType.Always, "常に確認する"),
			};
			this.TaskbarProgressFeatures = EnumerableEx
				.Return(GeneralSettings.TaskbarProgressSource.ToDefaultDisplay("使用しない"))
				.Concat(TaskbarProgress.Features.ToDisplay(x => x.Id, x => x.DisplayName))
				.ToList();
		}

		public void Initialize()
		{
			this.settings = SettingsHost.Instance<KanColleWindowSettings>();
			this.settings?.IsSplit.Subscribe(x => this.IsSplit = x).AddTo(this);
			this.settings?.Dock.Subscribe(x => this.Dock = x).AddTo(this);
			this.settings?.IsDivide.Subscribe(x => this.IsDivide = x).AddTo(this);
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
				this.settings.Dock.Value = this.Dock;
				this.settings.IsDivide.Value = this.IsDivide;
			}
		}

		public void Cancel()
		{
			if (this.settings != null)
			{
				this.IsSplit = this.settings.IsSplit;
				this.Dock = this.settings.Dock;
				this.IsDivide = this.settings.IsDivide;
			}
		}
	}
}
