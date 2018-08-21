using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Grabacr07.KanColleViewer.Models.Cef;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Win32;
using Nekoxy;

namespace Grabacr07.KanColleViewer.Models
{
	internal static class Helper
	{
		static Helper()
		{
			var version = Environment.OSVersion.Version;
			IsWindows8OrGreater = (version.Major == 6 && version.Minor >= 2) || version.Major > 6;
		}


		/// <summary>
		/// Windows 8 またはそれ以降のバージョンで動作しているかどうかを確認します。
		/// </summary>
		public static bool IsWindows8OrGreater { get; private set; }

		/// <summary>
		/// デザイナーのコンテキストで実行されているかどうかを取得します。
		/// </summary>
		public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
		
		public static string CreateScreenshotFilePath(SupportedImageFormat format)
		{
			var filePath = Path.Combine(
				ScreenshotSettings.Destination,
				$"KanColle-{DateTimeOffset.Now.LocalDateTime:yyMMdd-HHmmssff}");

			return Path.ChangeExtension(filePath, format.ToExtension());
		}

		public static void SetMMCSSTask()
		{
			var index = 0u;
			NativeMethods.AvSetMmThreadCharacteristics("Games", ref index);
		}

		public static void DeleteCacheIfRequested()
		{
			if (GeneralSettings.ClearCacheOnNextStartup)
			{
				try
				{
					Directory.Delete(CefBridge.CachePath, true);
					GeneralSettings.ClearCacheOnNextStartup.Value = false;
				}
				catch (Exception ex)
				{
					Application.TelemetryClient.TrackException(ex);
				}
			}
		}


		public static Color StringToColor(string colorCode)
		{
			try
			{
				if (colorCode.StartsWith("#"))
				{
					if (colorCode.Length == 7)
					{
						// #rrggbb style
						return Color.FromRgb(
							Convert.ToByte(colorCode.Substring(1, 2), 16),
							Convert.ToByte(colorCode.Substring(3, 2), 16),
							Convert.ToByte(colorCode.Substring(5, 2), 16));
					}
					if (colorCode.Length == 9)
					{
						// #aarrggbb style
						return Color.FromArgb(
							Convert.ToByte(colorCode.Substring(1, 2), 16),
							Convert.ToByte(colorCode.Substring(3, 2), 16),
							Convert.ToByte(colorCode.Substring(5, 2), 16),
							Convert.ToByte(colorCode.Substring(7, 2), 16));
					}
				}
			}
			catch (Exception ex)
			{
				// 雑
				System.Diagnostics.Debug.WriteLine(ex);
			}

			return Colors.Transparent;
		}

		public static HttpClientHandler GetProxyConfiguredHandler()
		{
			switch (HttpProxy.UpstreamProxyConfig.Type)
			{
				case ProxyConfigType.DirectAccess:
					return new HttpClientHandler
					{
						UseProxy = false,
					};

				case ProxyConfigType.SpecificProxy:
					return new HttpClientHandler
					{
						UseProxy = true,
						Proxy = new WebProxy($"{HttpProxy.UpstreamProxyConfig.SpecificProxyHost}:{HttpProxy.UpstreamProxyConfig.SpecificProxyPort}"),
					};

				case ProxyConfigType.SystemProxy:
					return new HttpClientHandler();

				default:
					return new HttpClientHandler();
			}
		}
	}
}
