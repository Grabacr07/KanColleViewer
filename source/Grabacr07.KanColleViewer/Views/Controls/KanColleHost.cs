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
using Grabacr07.KanColleViewer.ViewModels;
using MetroRadiance.Interop;
using MetroTrilithon.UI.Controls;
using mshtml;
using SHDocVw;
using IViewObject = Grabacr07.KanColleViewer.Win32.IViewObject;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using WebBrowser = System.Windows.Controls.WebBrowser;
using KanColleSettings = Grabacr07.KanColleViewer.Models.Settings.KanColleSettings;

namespace Grabacr07.KanColleViewer.Views.Controls
{
	[ContentProperty(nameof(WebBrowser))]
	[TemplatePart(Name = PART_ContentHost, Type = typeof(ScrollViewer))]
	public class KanColleHost : Control
	{
		private const string PART_ContentHost = "PART_ContentHost";

		public static Size KanColleSize { get; } = new Size(800.0, 480.0);
		public static Size InitialSize { get; } = new Size(800.0, 480.0);

		static KanColleHost()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(KanColleHost), new FrameworkPropertyMetadata(typeof(KanColleHost)));
		}

		private ScrollViewer scrollViewer;
		private bool styleSheetApplied;
		private Dpi? systemDpi;
		private bool firstLoaded;

		#region WebBrowser 依存関係プロパティ

		public WebBrowser WebBrowser
		{
			get { return (WebBrowser)this.GetValue(WebBrowserProperty); }
			set { this.SetValue(WebBrowserProperty, value); }
		}

		public static readonly DependencyProperty WebBrowserProperty =
			DependencyProperty.Register(nameof(WebBrowser), typeof(WebBrowser), typeof(KanColleHost), new UIPropertyMetadata(null, WebBrowserPropertyChangedCallback));

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
				var events = WebBrowserHelper.GetAxWebbrowser2(newBrowser) as DWebBrowserEvents_Event;
				if (events != null) events.NewWindow += instance.HandleWebBrowserNewWindow;
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
			KanColleSettings.FlashElementQuality.ValueChanged += (s, e) => ApplyFlashQuality(true);
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
				var provider = target.WebBrowser.Document as IServiceProvider;
				if (provider == null) return;

				object ppvObject;
				provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
				var webBrowser = ppvObject as IWebBrowser2;
				if (webBrowser == null) return;

				object pvaIn = zoomFactor;
				webBrowser.ExecWB(OLECMDID.OLECMDID_OPTICAL_ZOOM, OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn);
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

		private void HandleLoadCompleted(object sender, NavigationEventArgs e)
		{
			this.ApplyStyleSheet();
			this.ApplyFlashQuality();
			WebBrowserHelper.SetScriptErrorsSuppressed(this.WebBrowser, true);

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
		private void ApplyFlashQuality(bool ReloadRequired = false)
		{
			if (!this.firstLoaded) return;

			try
			{
				var document = this.WebBrowser.Document as HTMLDocument;
				if (document == null) return;

				var frames = document.frames;
				for (var i = 0; i < frames.length; i++)
				{
					var item = frames.item(i);
					var provider = item as IServiceProvider;
					if (provider == null) continue;

					object ppvObject;
					provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
					var webBrowser = ppvObject as IWebBrowser2;

					var iframeDocument = webBrowser?.Document as HTMLDocument;
					if (iframeDocument == null) continue;

					string qualityString = "high";
					switch (KanColleSettings.FlashElementQuality.Value)
					{
						case FlashQuality.Low: qualityString = "low"; break;
						case FlashQuality.Medium: qualityString = "medium"; break;
						case FlashQuality.High: qualityString = "high"; break;
					}

					var script = "";
					script += "function kcsFlash_StartFlash(a){var b={id:'externalswf',width:'800',height:'480',wmode:'opaque',quality:'" + qualityString + "',bgcolor:'#000000',allowScriptAccess:'always'};document.getElementById('flashWrap').innerHTML=ConstMessageInfo.InstallFlashMessage,gadgets.flash.embedFlash(a+ConstURLInfo.MainFlashURL+'?api_token='+flashInfo.apiToken+'&api_starttime='+flashInfo.apiStartTime,document.getElementById('flashWrap'),6,b),document.getElementById('adFlashWrap').style.height='0px',document.getElementById('wsFlashWrap').style.height='0px',document.getElementById('flashWrap').style.height='480px',gadgets.window.adjustHeight(ConstGadgetInfo.height)};";
					if(ReloadRequired) script += "kcsLogin_StartLogin();";

					webBrowser.Navigate("javascript:" + script);

					GCWorker.GCRequest();
				}
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
