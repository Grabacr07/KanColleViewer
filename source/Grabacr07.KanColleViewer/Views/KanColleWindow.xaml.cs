using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Grabacr07.KanColleViewer.Models.Settings;
using Livet;
using MetroTrilithon.Lifetime;
using MetroTrilithon.UI.Controls;

namespace Grabacr07.KanColleViewer.Views
{
	partial class KanColleWindow : IDisposableHolder
	{
		private static readonly Size informationDefaultSize = new Size(566, 358);
		private readonly KanColleWindowSettings settings;
		private readonly LivetCompositeDisposable compositeDisposable = new LivetCompositeDisposable();
		private Size? previousBrowserSize;
		private Dock? previousDock;

		public KanColleWindow()
		{
			this.InitializeComponent();

			this.settings = SettingsHost.Instance<KanColleWindowSettings>();
			this.settings.IsSplit.Subscribe(_ => this.ChangeSizeByDock()).AddTo(this);
			this.settings.AlwaysTopView.Subscribe(_ => this.ChangeSizeByDock()).AddTo(this);
			this.settings.Dock.Subscribe(_ => this.ChangeSizeByDock()).AddTo(this);
		}

		private void HandleKanColleHostSizeChangeRequested(object sender, Size size)
		{
			this.ChangeSizeByBrowser(size);
		}

		// 줌 크기 변경에 의한 윈도우 크기 재계산
		private void ChangeSizeByBrowser(Size browserSize)
		{
			if (!this.settings.AutomaticallyResize) return;

			// 사이즈 계산을 위해 일단 제한을 푼다.
			this.MinWidth = .0;
			this.MinHeight = .0;

			if (this.previousBrowserSize != null)
			{
				if (this.previousDock == Dock.Top || this.previousDock == Dock.Bottom) // 정보 영역 위 혹은 아래
				{
					var diffW = this.previousBrowserSize.Value.Width - browserSize.Width;
					if (Math.Abs(diffW) > 0.00001) this.Width -= diffW;
				}
				else if (this.previousDock == Dock.Left || this.previousDock == Dock.Right) // 정보영역 왼쪽 혹은 오른쪽
				{
					var diffH = this.previousBrowserSize.Value.Height - browserSize.Height;
					if (Math.Abs(diffH) > 0.00001) this.Height -= diffH;
				}
			}
			else
			{
				if (this.previousDock == Dock.Top || this.previousDock == Dock.Bottom)
				{
					this.Height += browserSize.Height;
				}
				else if (this.previousDock == Dock.Left || this.previousDock == Dock.Right)
				{
					this.Width += browserSize.Width;
				}
			}

			this.MinWidth = browserSize.Width + this.BorderThickness.Left + this.BorderThickness.Right;
			this.MinHeight = browserSize.Height + this.TopToolbarArea.ActualHeight + this.BottomToolbarArea.ActualHeight + this.captionBar.ActualHeight + this.statusBar.ActualHeight;

			this.previousBrowserSize = browserSize;
		}

		// 정보 영역 변경에 의한 윈도우 크기 재계산
		private void ChangeSizeByDock()
		{
			this.ChangeSizeByDock(this.settings.IsSplit ? (Dock?)null : this.settings.Dock.Value.Reverse());
		}

		// 정보 영역 변경에 의한 윈도우 크기 재계산 (실제 수행)
		private void ChangeSizeByDock(Dock? dock)
		{
			if (!this.settings.AutomaticallyResize) return;

			var browserSize = this.previousBrowserSize ?? new Size();

			var width = browserSize.Width;
			var height = browserSize.Height + this.TopToolbarArea.ActualHeight + this.captionBar.ActualHeight + this.statusBar.ActualHeight;

			if (dock == Dock.Top || dock == Dock.Bottom)
			{
				height += Math.Max(this.informationArea.ActualHeight, informationDefaultSize.Height);
			}
			else if (dock == Dock.Left || dock == Dock.Right)
			{
				width += Math.Max(this.informationArea.ActualWidth, informationDefaultSize.Width);
			}

			this.Width = width;
			this.Height = height;

			this.previousDock = dock;
		}

		ICollection<IDisposable> IDisposableHolder.CompositeDisposable => this.compositeDisposable;

		void IDisposable.Dispose()
		{
			this.compositeDisposable.Dispose();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			((IDisposable)this).Dispose();
		}
	}
}
