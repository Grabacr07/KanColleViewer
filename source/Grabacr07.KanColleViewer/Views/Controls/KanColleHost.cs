using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using CefSharp;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Cef;

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

		#region WebBrowser 依存関係プロパティ

		public ChromiumWebBrowser WebBrowser
		{
			get => (ChromiumWebBrowser)this.GetValue(WebBrowserProperty);
			set => this.SetValue(WebBrowserProperty, value);
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
				newBrowser.MenuHandler = new ContextMenuHandler();
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
			get => (double)this.GetValue(ZoomFactorProperty);
			set => this.SetValue(ZoomFactorProperty, value);
		}

		/// <summary>
		/// <see cref="ZoomFactor"/> 依存関係プロパティを識別します。
		/// </summary>
		public static readonly DependencyProperty ZoomFactorProperty =
			DependencyProperty.Register(nameof(ZoomFactor), typeof(double), typeof(KanColleHost), new UIPropertyMetadata(1.0, ZoomFactorChangedCallback));

		private static void ZoomFactorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;

			instance.ApplySize();
		}

		#endregion

		#region UserStyleSheet 依存関係プロパティ

		/// <summary>
		/// ユーザー スタイル シートを取得または設定します。
		/// </summary>
		public string UserStyleSheet
		{
			get => (string)this.GetValue(UserStyleSheetProperty);
			set => this.SetValue(UserStyleSheetProperty, value);
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
			this.Loaded += (sender, args) => this.ApplySize();
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

		public void ApplySize()
		{
			if (this.WebBrowser == null) return;

			this.ApplyZoomFactor(this.ZoomFactor);

			var baseSize = this.styleSheetApplied ? KanColleSize : InitialSize;
			var size = new Size(
				(baseSize.Width * this.ZoomFactor),
				(baseSize.Height * this.ZoomFactor));

			this.WebBrowser.Width = size.Width;
			this.WebBrowser.Height = size.Height;

			this.OwnerSizeChangeRequested?.Invoke(this, size);
		}

		private void ApplyZoomFactor(double zoomFactor)
		{
			if (zoomFactor < 0.1 || zoomFactor > 10)
			{
				StatusService.Current.Notify(string.Format(Properties.Resources.ZoomAction_OutOfRange, (int)(zoomFactor * 100)));
				return;
			}

			try
			{
				this.WebBrowser.ZoomLevel = 0;
				this.WebBrowser.ZoomLevel = Math.Log(zoomFactor) / Math.Log(1.2);
			}
			catch (Exception) when (Application.Instance.State == ApplicationState.Startup)
			{
				// about:blank だから仕方ない
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify(string.Format(Properties.Resources.ZoomAction_ZoomFailed, ex.Message));
				Application.TelemetryClient.TrackException(ex);
			}
		}

		private void HandleLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			if (e.Frame.IsMain)
			{
				this.Dispatcher.Invoke(() => this.ApplySize());
			}

			if (e.Url.Contains("/kcs2/index.php"))
			{
				this.Dispatcher.Invoke(() => this.ApplyStyleSheet());
			}
		}

		private void ApplyStyleSheet()
		{
			try
			{
				if (this.WebBrowser.TryGetKanColleCanvas(out _))
				{
					var js = $"var style = document.createElement(\"style\"); style.innerHTML = \"{this.UserStyleSheet.Replace("\r\n", " ")}\"; document.body.appendChild(style);";
					this.WebBrowser.GetMainFrame().ExecuteJavaScriptAsync(js);

					this.styleSheetApplied = true;
				}
			}
			catch (Exception) when (Application.Instance.State == ApplicationState.Startup)
			{
				// about:blank だから仕方ない
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify("failed to apply css: " + ex.Message);
				Application.TelemetryClient.TrackException(ex);
			}
		}
	}
}
