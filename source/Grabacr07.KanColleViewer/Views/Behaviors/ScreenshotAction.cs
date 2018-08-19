using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	/// <summary>
	/// 艦これのゲーム部分を画像として保存する機能を提供します。
	/// </summary>
	internal class ScreenshotAction : InteractionMessageAction<ChromiumWebBrowserWithScreenshotSupport>
	{
		protected override async void InvokeAction(InteractionMessage message)
		{
			var screenshotMessage = message as ScreenshotMessage;
			if (screenshotMessage == null)
			{
				return;
			}
			
			try
			{
				await this.TakeScreenshot(screenshotMessage.Path);
				StatusService.Current.Notify(Resources.Screenshot_Saved + Path.GetFileName(screenshotMessage.Path));
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify(Resources.Screenshot_Failed + ex.Message);
			}
		}

		private async Task TakeScreenshot(string path)
		{
			const string notFoundMessage = "艦これの HTML 要素が見つかりません。";
			const string script = "(function() { var element = document.getElementById('game_frame'); return [element.clientWidth, element.clientHeight]; })();";

			var frame = this.AssociatedObject.GetBrowser()?.MainFrame;
			if (frame == null) throw new Exception(notFoundMessage);

			var response = await frame.EvaluateScriptAsync(script);
			if (!response.Success) throw new Exception(notFoundMessage);

			var list = (List<object>)response.Result;
			var size = new Size((int)list[0], (int)list[1]);

			var bitmap = await this.AssociatedObject.TakeScreenshot(size, timeout: TimeSpan.FromSeconds(5));
			using (var stream = new FileStream(path, FileMode.Create))
			{
				var encoder = Path.GetExtension(path) == ".jpg"
					? (BitmapEncoder)new JpegBitmapEncoder()
					: new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(bitmap));
				encoder.Save(stream);
			}
		}
	}
}
