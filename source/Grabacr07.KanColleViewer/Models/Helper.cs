using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Grabacr07.KanColleViewer.Win32;
using Microsoft.Win32;
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


		public static string CreateScreenshotFilePath()
		{
			var filePath = Path.Combine(
				Settings.ScreenshotSettings.Destination,
				$"KanColle-{DateTimeOffset.Now.LocalDateTime.ToString("yyMMdd-HHmmssff")}");

			filePath = Path.ChangeExtension(
				filePath,
				Settings.ScreenshotSettings.Format == SupportedImageFormat.Png ? ".png" : ".jpg");

			return filePath;
		}


		/// <summary>
		/// HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
		/// に WebBrowser コントロールの動作バージョンを設定します。
		/// </summary>
		public static void SetRegistryFeatureBrowserEmulation()
		{
			const string key = @"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";

			try
			{
				var valueName = Process.GetCurrentProcess().ProcessName + ".exe";
				Registry.SetValue(key, valueName, Properties.Settings.Default.FeatureBrowserEmulation, RegistryValueKind.DWord);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public static void SetMMCSSTask()
		{
			var index = 0u;
			NativeMethods.AvSetMmThreadCharacteristics("Games", ref index);
		}


		/// <summary>
		/// キャッシュを削除します。
		/// </summary>
		/// <seealso cref="http://support.microsoft.com/kb/326201/ja"/>
		public static Task<bool> DeleteInternetCache()
		{
			return Task.Run(() => DeleteInternetCacheCore());
		}

		private static bool DeleteInternetCacheCore()
		{
			try
			{
				// TODO Chromiumを止めて、キャッシュをフォルダごと消す。プレイ中に止めるのはまずいので、終了時に削除とか…？
				// Directory.Delete(Path.Combine(Views.Controls.KanColleHost.CachePath, "Cache"), true);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				return false;
			}
			return true;
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
