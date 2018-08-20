using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet.Behaviors.Messaging;
using Livet.Messaging;
using Size = System.Windows.Size;

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
				await this.TakeScreenshotFromCanvas(screenshotMessage.Path, screenshotMessage.Format);
				StatusService.Current.Notify(Resources.Screenshot_Saved + Path.GetFileName(screenshotMessage.Path));
			}
			catch (Exception ex)
			{
				StatusService.Current.Notify(Resources.Screenshot_Failed + ex.Message);
			}
		}

		private async Task TakeScreenshotFromCef(string path, SupportedImageFormat format)
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
				var encoder = format.ToBitmapEncoder();
				if (encoder == null) throw new NotSupportedException($"{format} 形式はサポートされていません。");

				encoder.Frames.Add(BitmapFrame.Create(bitmap));
				encoder.Save(stream);
			}
		}

		private async Task TakeScreenshotFromCanvas(string path, SupportedImageFormat format)
		{
			var browser = this.AssociatedObject.GetBrowser();
			var gameFrame = browser.GetFrame("game_frame");
			var target = browser.GetFrameIdentifiers()
				.Select(x => browser.GetFrame(x))
				.Where(x => x.Parent?.Identifier == gameFrame.Identifier)
				.FirstOrDefault(x => x.Url.Contains("/kcs2/index.php"));
			if (target == null) throw new Exception("艦これの Canvas 要素が見つかりません。");

			var mimeType = format.ToMimeType();
			if (string.IsNullOrEmpty(mimeType)) throw new NotSupportedException($"{format} 形式はサポートされていません。");

			var check = await target.EvaluateScriptAsync("PIXI.settings.RENDER_OPTIONS.preserveDrawingBuffer");

			var toDataUrl = await target.EvaluateScriptAsync($"document.getElementsByTagName('canvas')[0].toDataURL('{mimeType}')");
			if (!toDataUrl.Success || !(toDataUrl.Result is string dataUrl)) throw new Exception(toDataUrl.Message);

			var array = dataUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2) throw new Exception($"無効な形式: {dataUrl}");

			var base64 = array[1];
			var bytes = Convert.FromBase64String(base64);

			using (var ms = new MemoryStream(bytes))
			{
				var image = System.Drawing.Image.FromStream(ms);
				image.Save(path, ImageFormat.Png);
			}
		}
	}
}
