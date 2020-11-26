using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Wpf;

namespace Grabacr07.KanColleViewer.Models.Cef
{
	public static class CefBridge
	{
		private static readonly string asesmblyDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
		private static readonly string cefDirectory = Path.Combine(asesmblyDirectory, Environment.Is64BitProcess ? "x64" : "x86");
		private static bool initialized;

		public static string CachePath => Path.Combine(Application.Instance.LocalAppData.FullName, "Chromium");

		public static void Initialize()
		{
			if (initialized) return;

			CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

			var cefSettings = new CefSettings()
			{
				BrowserSubprocessPath = Path.Combine(cefDirectory, "CefSharp.BrowserSubprocess.exe"),
				CachePath = CefBridge.CachePath,
			};
			cefSettings.CefCommandLineArgs.Add("disable-features", "AudioServiceOutOfProcess");
			cefSettings.CefCommandLineArgs.Add("proxy-server", Settings.NetworkSettings.LocalProxySettingsString);

			CefSharpSettings.SubprocessExitIfParentProcessClosed = true;
			CefSharp.Cef.Initialize(cefSettings);

			initialized = true;
		}

		public static Assembly ResolveCefSharpAssembly(object sender, ResolveEventArgs args)
		{
			if (args.Name.StartsWith("CefSharp"))
			{
				var assemblyName = args.Name.Split(new[] { ',' }, 2).FirstOrDefault() + ".dll";
				var archSpecificPath = Path.Combine(cefDirectory, assemblyName);

				return File.Exists(archSpecificPath)
					? Assembly.LoadFile(archSpecificPath)
					: null;
			}

			return null;
		}

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
	}
}
