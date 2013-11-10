using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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

		private void SaveCore(string path)
		{
			const string notFoundMessage = "艦これの Flash 要素が見つかりません。";

			var document = this.AssociatedObject.Document as HTMLDocument;
			if (document == null)
			{
				throw new Exception(notFoundMessage);
			}

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

				var target = iframeDocument.getElementById("externalswf") as HTMLEmbed;
				if (target == null) continue;

				var viewObject = target as IViewObject;
				if (viewObject == null) continue;

				find = true;

				var width = int.Parse(target.width);
				var height = int.Parse(target.height);

				var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
				var rect = new RECT { left = 0, top = 0, width = width, height = height, };
				var tdevice = new DVTARGETDEVICE { tdSize = 0, };

				using (var graphics = Graphics.FromImage(image))
				{
					var hdc = graphics.GetHdc();
					viewObject.Draw(1, 0, IntPtr.Zero, tdevice, IntPtr.Zero, hdc, rect, null, IntPtr.Zero, IntPtr.Zero);
					graphics.ReleaseHdc(hdc);
				}

				image.Save(path, ImageFormat.Png);
				break;
			}

			if (!find)
			{
				throw new Exception(notFoundMessage);
			}
		}
	}
}
