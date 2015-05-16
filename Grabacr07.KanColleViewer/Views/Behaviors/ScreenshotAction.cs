using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleViewer.Win32;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using mshtml;
using SHDocVw;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using WebBrowser = System.Windows.Controls.WebBrowser;
using Setting = Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Views.Controls;
using System.Diagnostics;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	/// <summary>
	/// 艦これの Flash 部分を画像として保存する機能を提供します。
	/// </summary>
	internal class ScreenshotAction : InteractionMessageAction<WebBrowser>
	{
		protected override void InvokeAction(InteractionMessage message)
		{
			var screenshotMessage = message as ScreenshotMessage;
			if (screenshotMessage == null)
			{
				return;
			}

			try
			{
				this.SaveCore(screenshotMessage.Path);
				screenshotMessage.Response = new Processing();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				KanColleClient.Current.CatchedErrorLogWriter.ReportException(ex.Source, ex);
				try
				{
					screenshotMessage.Response = new Processing(ex);
					TakeScreenshot(800, 480, null, screenshotMessage.Path);
				}
				catch (Exception e)
				{
					screenshotMessage.Response = new Processing(e);
				}
			}
		}


		/// <summary>
		/// <see cref="WebBrowser.Document"/> (<see cref="HTMLDocument"/>) から艦これの Flash 要素を特定し、指定したパスにスクリーンショットを保存します。
		/// </summary>
		/// <remarks>
		/// 本スクリーンショット機能は、「艦これ 司令部室」開発者の @haxe さんより多大なご協力を頂き実装できました。
		/// ありがとうございました。
		/// </remarks>
		/// <param name="path"></param>
		private void SaveCore(string path)
		{
			const string notFoundMessage = "칸코레 Flash를 찾을수 없습니다. 강제캡처모드로 캡처합니다.";

			var document = this.AssociatedObject.Document as HTMLDocument;
			if (document == null)
			{
				TakeScreenshot(800, 480, null, path);
				throw new Exception(notFoundMessage);
			}

			if (document.url.Contains(".swf?"))
			{
				var viewObject = document.getElementsByTagName("embed").item(0, 0) as IViewObject;
				if (viewObject == null)
				{
					TakeScreenshot(800, 480, null, path);
					throw new Exception(notFoundMessage);
				}

				var width = ((HTMLEmbed)viewObject).clientWidth;
				var height = ((HTMLEmbed)viewObject).clientHeight;
				//TakeScreenshot(width, height, viewObject, path);
				TakeScreenshot(width, height, null, path);
			}
			else
			{
				var gameFrame = document.getElementById("game_frame").document as HTMLDocument;
				if (gameFrame == null)
				{
					TakeScreenshot(800, 480, null, path);
					throw new Exception(notFoundMessage);
				}

				var frames = document.frames;
				var find = false;
				for (var i = 0; i < frames.length; i++)
				{
					var item = frames.item(i);
					var provider = item as IServiceProvider;
					if (provider == null) continue;

					object ppvObject;
					provider.QueryService(typeof(IWebBrowserApp).GUID, typeof(IWebBrowser2).GUID, out ppvObject);
					var webBrowser = ppvObject as IWebBrowser2;
					if (webBrowser == null) continue;

					var iframeDocument = webBrowser.Document as HTMLDocument;
					if (iframeDocument == null) continue;

					//flash要素が<embed>である場合と<object>である場合を判別して抽出
					IViewObject viewObject = null;
					int width = 0, height = 0;
					var swf = iframeDocument.getElementById("externalswf");
					if (swf == null) continue;
					Func<dynamic, bool> function = target =>
					{
						if (target == null) return false;
						viewObject = target as IViewObject;
						if (viewObject == null) return false;
						width = int.Parse(target.width);
						height = int.Parse(target.height);
						return true;
					};
					if (!function(swf as HTMLEmbed) && !function(swf as HTMLObjectElement)) continue;

					find = true;
					//TakeScreenshot(width, height, viewObject, path);
					TakeScreenshot(width, height, null, path);

					break;
				}

				if (!find)
				{
					TakeScreenshot(800, 480, null, path);
					throw new Exception(notFoundMessage);
				}
			}


		}

		private static void TakeScreenshot(int width, int height, IViewObject viewObject, string path)
		{
			var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			var rect = new RECT { left = 0, top = 0, width = width, height = height, };
			var tdevice = new DVTARGETDEVICE { tdSize = 0, };

			if (viewObject != null)
			{
				using (var graphics = Graphics.FromImage(image))
				{
					var hdc = graphics.GetHdc();
					viewObject.Draw(1, 0, IntPtr.Zero, tdevice, IntPtr.Zero, hdc, rect, null, IntPtr.Zero, IntPtr.Zero);
					graphics.ReleaseHdc(hdc);
				}
			}
			else
			{
				//From GrandcypherGear TakeScreenShot
				rect.height = Convert.ToInt32(rect.height * Setting.Current.BrowserZoomFactorPercentage / 100);
				rect.width = Convert.ToInt32(rect.width * Setting.Current.BrowserZoomFactorPercentage / 100);
				image = new Bitmap(rect.width, rect.height, PixelFormat.Format24bppRgb);

				KanColleViewer.Views.MainWindow.Current.SetPos();
				int left = KanColleViewer.Views.MainWindow.Current.rect.left;
				int top = KanColleViewer.Views.MainWindow.Current.rect.top + 91;


				if (Setting.Current.OrientationMode == OrientationType.Vertical)
				{
					top = top - 55;
				}

				using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
				{
					float dpiX = graphics.DpiX;
					float dpiY = graphics.DpiY;

					if (dpiX != 96f)
					{
						left = Convert.ToInt32(left * dpiX / 96f);
					}
					if (dpiY != 96f)
					{
						top = Convert.ToInt32(top * dpiY / 96f);
					}
				}
				if (left < 0) left = 0;
				using (var graphics = Graphics.FromImage(image))
				{
					graphics.CopyFromScreen(new System.Drawing.Point(left, top), new System.Drawing.Point(0, 0), new System.Drawing.Size(rect.width, rect.height));
				}
			}

			var format = Path.GetExtension(path) == ".jpg"
				? ImageFormat.Jpeg
				: ImageFormat.Png;

			image.Save(path, format);
			if (viewObject == null)
				throw new Exception("강제캡처모드로 스크린샷을 작성합니다.");
		}
	}
}
