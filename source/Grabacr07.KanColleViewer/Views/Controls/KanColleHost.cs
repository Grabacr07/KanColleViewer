using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CefSharp;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using MetroRadiance.Interop;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	[ContentProperty(nameof(WebBrowser))]
	[TemplatePart(Name = PART_ContentHost, Type = typeof(ScrollViewer))]
	public class KanColleHost : Control
	{
		private const string PART_ContentHost = "PART_ContentHost";

		public static Size KanColleSize { get; } = new Size(1200.0, 720.0);
		public static Size InitialSize { get; } = new Size(1200.0, 720.0);

		static KanColleHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(KanColleHost), new FrameworkPropertyMetadata(typeof(KanColleHost)));
		}

		private ScrollViewer scrollViewer;
		private bool styleSheetApplied;
		private Dpi? systemDpi;
		private bool firstLoaded;

		#region WebBrowser 依存関係プロパティ

		public ChromiumWebBrowser WebBrowser
		{
			get { return (ChromiumWebBrowser)this.GetValue(WebBrowserProperty); }
			set { this.SetValue(WebBrowserProperty, value); }
		}

		public static readonly DependencyProperty WebBrowserProperty =
			DependencyProperty.Register(nameof(WebBrowser), typeof(ChromiumWebBrowser), typeof(KanColleHost), new UIPropertyMetadata(null, WebBrowserPropertyChangedCallback));

		private static void WebBrowserPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;
			var newBrowser = (ChromiumWebBrowser)e.NewValue;
			var oldBrowser = (ChromiumWebBrowser)e.OldValue;

			if (oldBrowser != null)
			{
				oldBrowser.FrameLoadEnd -= instance.HandleLoadEnd;
			}
			if (newBrowser != null)
			{
				newBrowser.FrameLoadEnd += instance.HandleLoadEnd;
			}
			if (instance.scrollViewer != null)
			{
				instance.scrollViewer.Content = newBrowser;
			}
		}

		#endregion

		#region ZoomFactor 依存関係プロパティ

		/// <summary>
		/// ブラウザーのズーム倍率を取得または設定します。
		/// </summary>
		public double ZoomFactor
		{
			get { return (double)this.GetValue(ZoomFactorProperty); }
			set { this.SetValue(ZoomFactorProperty, value); }
		}

		/// <summary>
		/// <see cref="ZoomFactor"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty ZoomFactorProperty =
			DependencyProperty.Register(nameof(ZoomFactor), typeof(double), typeof(KanColleHost), new UIPropertyMetadata(1.0, ZoomFactorChangedCallback));

		private static void ZoomFactorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;

			instance.Update();
		}

		#endregion

		#region UserStyleSheet 依存関係プロパティ

		/// <summary>
		/// ユーザー スタイル シートを取得または設定します。
		/// </summary>
		public string UserStyleSheet
		{
			get { return (string)this.GetValue(UserStyleSheetProperty); }
			set { this.SetValue(UserStyleSheetProperty, value); }
		}

		/// <summary>
		/// <see cref="UserStyleSheet"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty UserStyleSheetProperty =
			DependencyProperty.Register(nameof(UserStyleSheet), typeof(string), typeof(KanColleHost), new UIPropertyMetadata(string.Empty, UserStyleSheetChangedCallback));

		private static void UserStyleSheetChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;

			instance.ApplyStyleSheet();
		}

		#endregion

		public event EventHandler<Size> OwnerSizeChangeRequested;

		public KanColleHost()
		{
			this.Loaded += (sender, args) => this.Update();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.scrollViewer = this.GetTemplateChild(PART_ContentHost) as ScrollViewer;
			if (this.scrollViewer != null)
			{
				this.scrollViewer.Content = this.WebBrowser;
			}
		}

		public void Update()
		{
			if (this.WebBrowser == null) return;

			var dpi = this.systemDpi ?? (this.systemDpi = this.GetSystemDpi()) ?? Dpi.Default;
			var zoomFactor = dpi.ScaleX + (this.ZoomFactor - 1.0);
			var percentage = (int)(zoomFactor * 100);

			ApplyZoomFactor(this, percentage);

			var size = this.styleSheetApplied ? KanColleSize : InitialSize;
			size = new Size(
				(size.Width * (zoomFactor / dpi.ScaleX)) / dpi.ScaleX,
				(size.Height * (zoomFactor / dpi.ScaleY)) / dpi.ScaleY);
			this.WebBrowser.Width = size.Width;
			this.WebBrowser.Height = size.Height;

			this.OwnerSizeChangeRequested?.Invoke(this, size);
		}

		private static void ApplyZoomFactor(KanColleHost target, int zoomFactor)
		{
			if (!target.firstLoaded) return;

			if (zoomFactor < 10 || zoomFactor > 1000)
			{
				StatusService.Current.Notify(string.Format(Properties.Resources.ZoomAction_OutOfRange, zoomFactor));
				return;
			}

			try
			{
				target.WebBrowser.ZoomLevel = Math.Log(zoomFactor / 100.0) / Math.Log(1.2);
			}
			catch (Exception) when (Application.Instance.State == ApplicationState.Startup)
			{
				// about:blank だから仕方ない
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				StatusService.Current.Notify(string.Format(Properties.Resources.ZoomAction_ZoomFailed, ex.Message));
			}
		}

		private void HandleLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			this.Dispatcher.Invoke(() =>
			{
				this.ApplyStyleSheet();

				this.firstLoaded = true;
				this.Update();
			});
		}

		private void ApplyStyleSheet()
		{
			if (!this.firstLoaded) return;

			try
			{
				var frame = this.WebBrowser.GetMainFrame();
				if (frame == null) return;

				var js = $"var style = document.createElement(\"style\"); style.innerHTML = \"{this.UserStyleSheet.Replace("\r\n", " ")}\"; document.body.appendChild(style);";
				frame.ExecuteJavaScriptAsync(js);

				this.styleSheetApplied = true;
			}
			catch (Exception) when (Application.Instance.State == ApplicationState.Startup)
			{
				// about:blank だから仕方ない
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				StatusService.Current.Notify("failed to apply css: " + ex.Message);
			}
		}
	}
}
