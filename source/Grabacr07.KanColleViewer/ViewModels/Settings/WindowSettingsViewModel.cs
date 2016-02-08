using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
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

		public WindowSettingsViewModel()
		{
			this.ExitConfirmationTypes = new List<DisplayViewModel<ExitConfirmationType>>
			{
				DisplayViewModel.Create(ExitConfirmationType.None, Resources.Settings_Window_ConfirmExit_Never),
				DisplayViewModel.Create(ExitConfirmationType.InSortieOnly, Resources.Settings_Window_ConfirmExit_InSortieOnly),
				DisplayViewModel.Create(ExitConfirmationType.Always, Resources.Settings_Window_ConfirmExit_Always),
			};
			this.TaskbarProgressFeatures = EnumerableEx
				.Return(GeneralSettings.TaskbarProgressSource.ToDefaultDisplay(Resources.TaskbarIndicator_DoNotUse))
				.Concat(TaskbarProgress.Features.ToDisplay(x => x.Id, x => x.DisplayName))
				.ToList();
		}

		public void Initialize()
		{
			this.settings = SettingsHost.Instance<KanColleWindowSettings>();
			this.settings?.IsSplit.Subscribe(x => this.IsSplit = x).AddTo(this);
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
				this.settings.Dock.Value = this.Dock;
			}
		}

		public void Cancel()
		{
			if (this.settings != null)
			{
				this.IsSplit = this.settings.IsSplit;
				this.Dock = this.settings.Dock;
			}
		}
	}
}
