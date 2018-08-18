using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using CefSharp;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.ViewModels;
using MetroRadiance.Interop;
using MetroTrilithon.UI.Controls;
using MSHTML;
using SHDocVw;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
//using WebBrowser = System.Windows.Controls.WebBrowser;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	[ContentProperty(nameof(CWebBrowser))]
	[TemplatePart(Name = PART_ContentHost, Type = typeof(ScrollViewer))]
	public class KanColleHost : Control
	{
		private const string PART_ContentHost = "PART_ContentHost";

		public static Size KanColleSize { get; } = new Size(1200.0, 720.0);
		public static Size InitialSize { get; } = new Size(1200.0, 720.0);

		static KanColleHost()
		{
			CefSettings cefSettings = new CefSettings();
			cefSettings.CefCommandLineArgs.Add("proxy-server", "http=127.0.0.1:37564");
			Cef.Initialize(cefSettings);
			DefaultStyleKeyProperty.OverrideMetadata(typeof(KanColleHost), new FrameworkPropertyMetadata(typeof(KanColleHost)));
		}

		private ScrollViewer scrollViewer;
		private bool styleSheetApplied;
		private Dpi? systemDpi;
		private bool firstLoaded;

		#region CWebBrowser 依存関係プロパティ

		public ChromiumWebBrowser CWebBrowser
		{
			get { return (ChromiumWebBrowser)this.GetValue(CWebBrowserProperty); }
			set { this.SetValue(CWebBrowserProperty, value); }
		}

		public static readonly DependencyProperty CWebBrowserProperty =
			DependencyProperty.Register(nameof(CWebBrowser), typeof(ChromiumWebBrowser), typeof(KanColleHost), new UIPropertyMetadata(null, CWebBrowserPropertyChangedCallback));

		private static void CWebBrowserPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var instance = (KanColleHost)d;
			var newBrowser = (ChromiumWebBrowser)e.NewValue;
			var oldBrowser = (ChromiumWebBrowser)e.OldValue;

			if (oldBrowser != null)
			{
				oldBrowser.LoadingStateChanged -= instance.HandleLoadingStateChanged;
			}
			if (newBrowser != null)
			{
				newBrowser.LoadingStateChanged += instance.HandleLoadingStateChanged;
				//var events = WebBrowserHelper.GetAxWebbrowser2(newBrowser) as DWebBrowserEvents_Event;
				//if (events != null) events.NewWindow += instance.HandleWebBrowserNewWindow;
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
				this.scrollViewer.Content = this.CWebBrowser;
			}
		}

		public void Update()
		{
			if (this.CWebBrowser == null) return;

			var dpi = this.systemDpi ?? (this.systemDpi = this.GetSystemDpi()) ?? Dpi.Default;
			var zoomFactor = dpi.ScaleX + (this.ZoomFactor - 1.0);
			var percentage = (int)(zoomFactor * 100);

			ApplyZoomFactor(this, percentage);

			var size = this.styleSheetApplied ? KanColleSize : InitialSize;
			size = new Size(
				(size.Width * (zoomFactor / dpi.ScaleX)) / dpi.ScaleX,
				(size.Height * (zoomFactor / dpi.ScaleY)) / dpi.ScaleY);
			this.CWebBrowser.Width = size.Width;
			this.CWebBrowser.Height = size.Height;

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
				/*
				var provider = target.WebBrowser.Document as IServiceProvider;
				if (provider == null) return;

				object ppvObject;
				provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
				var webBrowser = ppvObject as IWebBrowser2;
				if (webBrowser == null) return;

				object pvaIn = zoomFactor;
				webBrowser.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn);
				*/
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

		private void HandleLoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
		{
			if (e.IsLoading == false)
			{
				Dispatcher.Invoke(() => {
					ApplyStyleSheet();
					firstLoaded = true;
					Update();
				});
			}
		}

		private void HandleLoadCompleted(object sender, RoutedEventArgs e)
		{
			this.ApplyStyleSheet();
			//WebBrowserHelper.SetScriptErrorsSuppressed(this.WebBrowser, true);

			this.firstLoaded = true;
			this.Update();
		}

		private void HandleWebBrowserNewWindow(string url, int flags, string targetFrameName, ref object postData, string headers, ref bool processed)
		{
			processed = true;

			var window = new BrowserWindow { DataContext = new NavigatorViewModel(), };
			window.Show();
			window.WebBrowser.Navigate(url);
		}

		private void ApplyStyleSheet()
		{
			if (!this.firstLoaded) return;

			try
			{
				/*
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

				var target = gameFrame?.document as HTMLDocument;
				if (target != null)
				{
					target.createStyleSheet().cssText = this.UserStyleSheet;
					this.styleSheetApplied = true;
				}
				*/
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
