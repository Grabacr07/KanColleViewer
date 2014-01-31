using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Grabacr07.KanColleViewer.Win32;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using mshtml;
using SHDocVw;
using IServiceProvider = Grabacr07.KanColleViewer.Win32.IServiceProvider;
using WebBrowser = System.Windows.Controls.WebBrowser;

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
				screenshotMessage.Response = new Processing(ex);
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
			const string notFoundMessage = "艦これの Flash 要素が見つかりません。";

			var document = this.AssociatedObject.Document as HTMLDocument;
			if (document == null)
			{
				throw new Exception(notFoundMessage);
			}

			if (document.url.Contains(".swf?"))
			{
				var viewObject = document.getElementsByTagName("embed").item(0, 0) as IViewObject;
				if (viewObject == null)
				{
					throw new Exception(notFoundMessage);
				}

				var width = ((HTMLEmbed)viewObject).clientWidth;
				var height = ((HTMLEmbed)viewObject).clientHeight;
				TakeScreenshot(width, height, viewObject, path);
			}
			else
			{
				var gameFrame = document.getElementById("game_frame").document as HTMLDocument;
				if (gameFrame == null)
				{
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
					TakeScreenshot(width, height, viewObject, path);

					break;
				}

				if (!find)
				{
					throw new Exception(notFoundMessage);
				}
			}


		}

		private static void TakeScreenshot(int width, int height, IViewObject viewObject, string path)
		{
			var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
			var rect = new RECT { left = 0, top = 0, width = width, height = height, };
			var tdevice = new DVTARGETDEVICE { tdSize = 0, };

			using (var graphics = Graphics.FromImage(image))
			{
				var hdc = graphics.GetHdc();
				viewObject.Draw(1, 0, IntPtr.Zero, tdevice, IntPtr.Zero, hdc, rect, null, IntPtr.Zero, IntPtr.Zero);
				graphics.ReleaseHdc(hdc);
			}

			var format = Path.GetExtension(path) == ".jpg"
				? ImageFormat.Jpeg 
				: ImageFormat.Png;

			image.Save(path, format);
		}
	}
}
