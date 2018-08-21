using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;

namespace Grabacr07.KanColleViewer.Models.Cef
{
	public static class CefBridge
	{
		public static string CachePath => Path.Combine(Application.Instance.LocalAppData.FullName, "Chromium");

		public static bool TryGetKanColleCanvas(this ChromiumWebBrowser webBrowser, out IFrame canvas)
		{
			var browser = webBrowser.GetBrowser();
			var gameFrame = browser.GetFrame("game_frame");
			if (gameFrame == null)
			{
				canvas = null;
				return false;
			}

			canvas = browser.GetFrameIdentifiers()
				.Select(x => browser.GetFrame(x))
				.Where(x => x.Parent?.Identifier == gameFrame.Identifier)
				.FirstOrDefault(x => x.Url.Contains("/kcs2/index.php"));

			return canvas != null;
		}

		public static Image DataUrlToImage(string dataUrl)
		{
			var array = dataUrl.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length != 2) throw new Exception($"無効な形式: {dataUrl}");

			var base64 = array[1];
			var bytes = Convert.FromBase64String(base64);

			using (var ms = new MemoryStream(bytes))
			{
				return Image.FromStream(ms);
			}
		}
	}
}
