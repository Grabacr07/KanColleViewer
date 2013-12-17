using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using Grabacr07.Desktop.Metro.Win32;

namespace Grabacr07.Desktop.Metro.Chrome
{
	/// <summary>
	/// Metro スタイル風のウィンドウを表します。
	/// </summary>
	public class MetroWindow : Window
	{
		static MetroWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (MetroWindow), new FrameworkPropertyMetadata(typeof (MetroWindow)));
		}


		private HwndSource source;

		/// <summary>
		/// WPF が認識しているシステムの DPI (プライマリ モニターの DPI)。
		/// </summary>
		private Dpi systemDpi;

		/// <summary>
		/// このウィンドウが表示されているモニターの現在の DPI。
		/// </summary>
		private Dpi currentDpi;

		#region DpiScaleTransform 依存関係プロパティ

		/// <summary>
		/// DPI スケーリングを実現する <see cref="Transform" /> を取得または設定します。
		/// </summary>
		public Transform DpiScaleTransform
		{
			get { return (Transform)this.GetValue(DpiScaleTransformProperty); }
			set { this.SetValue(DpiScaleTransformProperty, value); }
		}

		/// <summary>
		/// <see cref="DpiScaleTransform" /> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty DpiScaleTransformProperty =
			DependencyProperty.Register("DpiScaleTransform", typeof(Transform), typeof(MetroWindow), new UIPropertyMetadata(Transform.Identity));

		#endregion

		#region WindowChrome 依存関係プロパティ

		public WindowChrome WindowChrome
		{
			get { return (WindowChrome)this.GetValue(WindowChromeProperty); }
			set { this.SetValue(WindowChromeProperty, value); }
		}

		public static readonly DependencyProperty WindowChromeProperty =
			DependencyProperty.Register("WindowChrome", typeof(WindowChrome), typeof(MetroWindow), new UIPropertyMetadata(null));

		#endregion

		#region InactiveBorderBrush 依存関係プロパティ

		/// <summary>
		/// このウィンドウの非アクティブ時、ウィンドウの境界の塗りつぶしに使用するブラシを取得または設定します。
		/// </summary>
		public Brush InactiveBorderBrush
		{
			get { return (Brush)this.GetValue(InactiveBorderBrushProperty); }
			set { this.SetValue(InactiveBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty InactiveBorderBrushProperty =
			DependencyProperty.Register("InactiveBorderBrush", typeof(Brush), typeof(MetroWindow), new UIPropertyMetadata(null));

		#endregion

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			if (!PerMonitorDpiService.IsSupported) return;

			this.systemDpi = this.GetSystemDpi();

			this.source = PresentationSource.FromVisual(this) as HwndSource;
			if (this.source != null)
			{
				this.currentDpi = this.source.GetDpi();
				this.ChangeDpi(this.currentDpi);
				this.source.AddHook(this.WndProc);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if (this.source != null)
			{
				this.source.RemoveHook(this.WndProc);
			}
		}


		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.DPICHANGED)
			{
				var dpiX = wParam.ToHiWord();
				var dpiY = wParam.ToLoWord();
				this.ChangeDpi(new Dpi(dpiX, dpiY));
				handled = true;
			}

			return IntPtr.Zero;
		}

		private void ChangeDpi(Dpi dpi)
		{
			if (!PerMonitorDpiService.IsSupported) return;

			this.DpiScaleTransform = (dpi == this.systemDpi)
				? Transform.Identity
				: new ScaleTransform((double)dpi.X / this.systemDpi.X, (double)dpi.Y / this.systemDpi.Y);

			this.Width = this.Width * dpi.X / this.currentDpi.X;
			this.Height = this.Height * dpi.Y / this.currentDpi.Y;

			this.currentDpi = dpi;
		}
	}
}
