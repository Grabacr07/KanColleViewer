using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CefSharp;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Cef;
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
		private IFrame targetCanvas;

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
				if (encoder == null) throw new ArgumentException($"{format} 形式はサポートされていません。");

				encoder.Frames.Add(BitmapFrame.Create(bitmap));
				encoder.Save(stream);
			}
		}

		private Task TakeScreenshotFromCanvas(string path, SupportedImageFormat format)
		{
			var source = new TaskCompletionSource<Unit>();

			if (this.targetCanvas == null && !this.AssociatedObject.TryGetKanColleCanvas(out this.targetCanvas))
			{
				source.SetException(new Exception("艦これの Canvas 要素が見つかりません。"));
				return source.Task;
			}

			var request = new ScreenshotRequest(path, format, source);
			var script = $@"
(async function()
{{
	await CefSharp.BindObjectAsync('{request.Id}');

	var canvas = document.querySelector('canvas');
	requestAnimationFrame(() =>
	{{
		var dataUrl = canvas.toDataURL('{format.ToMimeType()}');
		{request.Id}.complete(dataUrl);
	}});
}})();
";
			this.AssociatedObject.JavascriptObjectRepository.Register(request.Id, request, true);
			this.targetCanvas.ExecuteJavaScriptAsync(script);

			return source.Task;
		}
	}

}
