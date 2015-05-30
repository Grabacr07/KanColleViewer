using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using Grabacr07.KanColleViewer.Models;
using MetroRadiance.Core;
using mshtml;
using SHDocVw;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	[ContentProperty("WebBrowser")]
	[TemplatePart(Name = PART_ContentHost, Type = typeof(ScrollViewer))]
	public class KanColleHost : Control
	{
		private const string PART_ContentHost = "PART_ContentHost";
		private static readonly Size kanColleSize = new Size(800.0, 480.0);
		private static readonly Size browserSize = new Size(960.0, 572.0);

		static KanColleHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(KanColleHost), new FrameworkPropertyMetadata(typeof(KanColleHost)));
		}

		private ScrollViewer scrollViewer;
		private bool styleSheetApplied;
		private Dpi? systemDpi;

		#region WebBrowser 依存関係プロパティ

		public WebBrowser WebBrowser
		{
			get { return (WebBrowser)this.GetValue(WebBrowserProperty); }
			set { this.SetValue(WebBrowserProperty, value); }
		}

		public static readonly DependencyProperty WebBrowserProperty =
			DependencyProperty.Register("WebBrowser", typeof(WebBrowser), typeof(KanColleHost), new UIPropertyMetadata(null, WebBrowserPropertyChangedCallback));

		private static void WebBrowserPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;
			var newBrowser = (WebBrowser)e.NewValue;
			var oldBrowser = (WebBrowser)e.OldValue;

			if (oldBrowser != null)
			{
				oldBrowser.LoadCompleted -= instance.HandleLoadCompleted;
			}
			if (newBrowser != null)
			{
				newBrowser.LoadCompleted += instance.HandleLoadCompleted;
			}
			if (instance.scrollViewer != null)
			{
				instance.scrollViewer.Content = newBrowser;
			}

			WebBrowserHelper.SetAllowWebBrowserDrop(newBrowser, false);
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
			DependencyProperty.Register("ZoomFactor", typeof(double), typeof(KanColleHost), new UIPropertyMetadata(1.0, ZoomFactorChangedCallback));

		private static void ZoomFactorChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;

			instance.Update();
		}

		#endregion


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

			ApplyZoomFactor(this.WebBrowser, percentage);

			if (this.styleSheetApplied)
			{
				this.WebBrowser.Width = (kanColleSize.Width * (zoomFactor / dpi.ScaleX)) / dpi.ScaleX;
				this.WebBrowser.Height = (kanColleSize.Height * (zoomFactor / dpi.ScaleY)) / dpi.ScaleY;
				this.MinWidth = this.WebBrowser.Width;
			}
			else
			{
				this.WebBrowser.Width = double.NaN;
				this.WebBrowser.Height = (browserSize.Height * (zoomFactor / dpi.ScaleY)) / dpi.ScaleY;
				this.MinWidth = (browserSize.Width * (zoomFactor / dpi.ScaleX)) / dpi.ScaleX;
			}
		}

		private static void ApplyZoomFactor(WebBrowser target, int zoomFactor)
		{
			if (zoomFactor < 10 || zoomFactor > 1000)
			{
				StatusService.Current.Notify(string.Format(Properties.Resources.ZoomAction_OutOfRange, zoomFactor));
				return;
			}

			try
			{
				var provider = target.Document as IServiceProvider;
				if (provider == null) return;

				object ppvObject;
				provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
				var webBrowser = ppvObject as IWebBrowser2;
				if (webBrowser == null) return;

				object pvaIn = zoomFactor;
				webBrowser.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				StatusService.Current.Notify(string.Format(Properties.Resources.ZoomAction_ZoomFailed, ex.Message));
			}
		}

		private void HandleLoadCompleted(object sender, NavigationEventArgs e)
		{
			this.ApplyStyleSheet();
			WebBrowserHelper.SetScriptErrorsSuppressed(this.WebBrowser, true);

			this.Update();

			var window = Window.GetWindow(this.WebBrowser);
			if (window != null)
			{
				window.Width = this.WebBrowser.Width;
			}
		}

		private void ApplyStyleSheet()
		{
			try
			{
				var document = this.WebBrowser.Document as HTMLDocument;
				if (document == null) return;

				var gameFrame = document.getElementById("game_frame");
				if (gameFrame == null)
				{
					if (document.url.Contains(".swf?"))
					{
						gameFrame = document.body;
					}
				}

				if (gameFrame != null)
				{
					var target = gameFrame.document as HTMLDocument;
					if (target != null)
					{
						target.createStyleSheet().cssText = Properties.Settings.Default.OverrideStyleSheet;
						this.styleSheetApplied = true;
						return;
					}
				}
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify("failed to apply css: " + ex.Message);
			}

			return;
		}
	}
}
