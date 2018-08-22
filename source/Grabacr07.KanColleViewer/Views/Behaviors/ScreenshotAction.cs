using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Cef;
using Grabacr07.KanColleViewer.Properties;
using Grabacr07.KanColleViewer.ViewModels.Messages;
using Livet.Behaviors.Messaging;
using Livet.Messaging;

namespace Grabacr07.KanColleViewer.Views.Behaviors
{
	/// <summary>
	/// 艦これのゲーム部分を画像として保存する機能を提供します。
	/// </summary>
	internal class ScreenshotAction : InteractionMessageAction<ChromiumWebBrowser>
	{
		private IFrame targetCanvas;

		protected override async void InvokeAction(InteractionMessage message)
		{
			if (message is ScreenshotMessage screenshotMessage)
			{
				try
				{
					await this.TakeScreenshot(screenshotMessage.Path, screenshotMessage.Format);
					StatusService.Current.Notify(Resources.Screenshot_Saved + Path.GetFileName(screenshotMessage.Path));
				}
				catch (Exception ex)
				{
					StatusService.Current.Notify(Resources.Screenshot_Failed + ex.Message);
					Application.TelemetryClient.TrackException(ex);
				}
			}
		}

		private Task TakeScreenshot(string path, SupportedImageFormat format)
		{
			var source = new TaskCompletionSource<Unit>();

			if (this.targetCanvas == null && !this.AssociatedObject.TryGetKanColleCanvas(out this.targetCanvas))
			{
				source.SetException(new Exception("艦これの Canvas 要素が見つかりません。"));
				return source.Task;
			}

			var request = new ScreenshotRequest(path, source);
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
