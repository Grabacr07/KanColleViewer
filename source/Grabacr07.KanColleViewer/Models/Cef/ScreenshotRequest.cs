using System;
using System.Reactive;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models.Cef
{
	public class ScreenshotRequest
	{
		private readonly TaskCompletionSource<Unit> source;
		private readonly string path;
		private readonly SupportedImageFormat format;

		public string Id { get; }

		public ScreenshotRequest(string path, SupportedImageFormat format, TaskCompletionSource<Unit> source)
		{
			this.Id = $"ssReq{DateTimeOffset.Now.Ticks}";
			this.path = path;
			this.format = format;
			this.source = source;
		}

		public void Complete(string dataUrl)
		{
			try
			{
				using (var image = CefBridge.DataUrlToImage(dataUrl))
				{
					image.Save(this.path, this.format.ToImageFormat());
				}

				this.source.SetResult(Unit.Default);
			}
			catch (Exception ex)
			{
				this.source.SetException(ex);
			}
		}
	}
}
