using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using Grabacr07.Desktop.Metro.Win32;

namespace Grabacr07.Desktop.Metro.Chrome
{
	/// <summary>
	/// ウィンドウの四辺にアタッチされる発行ウィンドウを表します。
	/// </summary>
	internal class GlowWindow : Window
	{
		static GlowWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof (GlowWindow), new FrameworkPropertyMetadata(typeof (GlowWindow)));
		}

		#region IsGlow 依存関係プロパティ

		public bool IsGlow
		{
			get { return (bool)this.GetValue(IsGlowProperty); }
			set { this.SetValue(IsGlowProperty, value); }
		}

		public static readonly DependencyProperty IsGlowProperty =
			DependencyProperty.Register("IsGlow", typeof (bool), typeof (GlowWindow), new UIPropertyMetadata(true));

		#endregion

		#region Orientation 依存関係プロパティ

		public Orientation Orientation
		{
			get { return (Orientation)this.GetValue(OrientationProperty); }
			set { this.SetValue(OrientationProperty, value); }
		}

		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof (Orientation), typeof (GlowWindow), new UIPropertyMetadata(Orientation.Horizontal));

		#endregion

		#region ActiveGlowBrush 依存関係プロパティ

		public Brush ActiveGlowBrush
		{
			get { return (Brush)this.GetValue(ActiveGlowBrushProperty); }
			set { this.SetValue(ActiveGlowBrushProperty, value); }
		}
		public static readonly DependencyProperty ActiveGlowBrushProperty =
			DependencyProperty.Register("ActiveGlowBrush", typeof(Brush), typeof(GlowWindow), new UIPropertyMetadata(null));

		#endregion

		#region InactiveGlowBrush 依存関係プロパティ

		public Brush InactiveGlowBrush
		{
			get { return (Brush)this.GetValue(InactiveGlowBrushProperty); }
			set { this.SetValue(InactiveGlowBrushProperty, value); }
		}
		public static readonly DependencyProperty InactiveGlowBrushProperty =
			DependencyProperty.Register("InactiveGlowBrush", typeof(Brush), typeof(GlowWindow), new UIPropertyMetadata(null));

		#endregion


		private IntPtr handle;
		private readonly GlowWindowProcessor processor;

		private readonly Window owner;
		private IntPtr ownerHandle;

		internal GlowWindow(Window owner, GlowWindowProcessor processor)
		{
			this.owner = owner;
			this.processor = processor;

			this.WindowStyle = WindowStyle.None;
			this.AllowsTransparency = true;
			this.ShowActivated = false;
			this.Visibility = Visibility.Collapsed;
			this.ResizeMode = ResizeMode.NoResize;
			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Left = processor.GetLeft(owner.Left, owner.ActualWidth);
			this.Top = processor.GetTop(owner.Top, owner.ActualHeight);
			this.Width = processor.GetWidth(owner.Left, owner.ActualWidth);
			this.Height = processor.GetHeight(owner.Top, owner.ActualHeight);
			this.Orientation = processor.Orientation;
			this.HorizontalContentAlignment = processor.HorizontalAlignment;
			this.VerticalContentAlignment = processor.VerticalAlignment;

			var bindingActive = new Binding("BorderBrush") { Source = this.owner, };
			this.SetBinding(ActiveGlowBrushProperty, bindingActive);

			var bindingInactive = new Binding("InactiveBorderBrush") { Source = this.owner, };
			this.SetBinding(InactiveGlowBrushProperty, bindingInactive);
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source != null)
			{
				this.handle = source.Handle;

				var wsex = this.handle.GetWindowLongEx();
				wsex ^= WSEX.APPWINDOW;
				wsex |= WSEX.NOACTIVATE;
				this.handle.SetWindowLongEx(wsex);

				var cs = this.handle.GetClassLong(ClassLongFlags.GclStyle);
				cs |= ClassStyles.DblClks;
				this.handle.SetClassLong(ClassLongFlags.GclStyle, cs);

				source.AddHook(this.WndProc);
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			var source = PresentationSource.FromVisual(this) as HwndSource;
			if (source != null)
			{
				source.RemoveHook(this.WndProc);
			}
		}

		public void Update()
		{
			if (this.owner.WindowState == WindowState.Normal)
			{
				this.UpdateCore();
				this.Visibility = Visibility.Visible;
			}
			else
			{
				this.Visibility = Visibility.Collapsed;
			}
		}

		private void UpdateCore()
		{
			if (this.ownerHandle == IntPtr.Zero)
			{
				this.ownerHandle = new WindowInteropHelper(this.owner).Handle;
			}

			this.IsGlow = this.owner.IsActive;

			var dpi = this.GetSystemDpi();
			var left = (int)Math.Round(processor.GetLeft(owner.Left, owner.ActualWidth) * dpi.ScaleX);
			var top = (int)Math.Round(processor.GetTop(owner.Top, owner.ActualHeight) * dpi.ScaleY);
			var width = (int)Math.Round(processor.GetWidth(owner.Left, owner.ActualWidth) * dpi.ScaleX);
			var height = (int)Math.Round(processor.GetHeight(owner.Top, owner.ActualHeight) * dpi.ScaleY);
			NativeMethods.SetWindowPos(this.handle, this.ownerHandle, left, top, width, height, SWP.NOACTIVATE);
		}


		private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == (int)WM.MOUSEACTIVATE)
			{
				handled = true;
				return new IntPtr(3);
			}

			if (msg == (int)WM.LBUTTONDOWN)
			{
				if (!this.owner.IsActive)
				{
					this.owner.Activate();
				}

				var ptScreen = lParam.ToPoint();

				NativeMethods.PostMessage(
					this.ownerHandle,
					(uint)WM.NCLBUTTONDOWN,
					(IntPtr)this.processor.GetHitTestValue(ptScreen, this.ActualWidth, this.ActualHeight),
					IntPtr.Zero);
			}
			if (msg == (int)WM.NCHITTEST)
			{
				var ptScreen = lParam.ToPoint();
				var ptClient = this.PointFromScreen(ptScreen);

				this.Cursor = this.processor.GetCursor(ptClient, this.ActualWidth, this.ActualHeight);
			}

			if (msg == (int)WM.LBUTTONDBLCLK)
			{
				if (this.processor.GetType() == typeof(GlowWindowProcessorTop))
				{
					NativeMethods.SendMessage(this.ownerHandle, WM.NCLBUTTONDBLCLK, (IntPtr)HitTestValues.HTTOP, IntPtr.Zero);
				}
				else if (this.processor.GetType() == typeof(GlowWindowProcessorBottom))
				{
					NativeMethods.SendMessage(this.ownerHandle, WM.NCLBUTTONDBLCLK, (IntPtr)HitTestValues.HTBOTTOM, IntPtr.Zero);
				}
			}

			return IntPtr.Zero;
		}
	}
}
