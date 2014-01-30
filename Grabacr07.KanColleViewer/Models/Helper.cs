using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace Grabacr07.KanColleViewer.Models
{
	internal static class Helper
	{
		static Helper()
		{
			var version = Environment.OSVersion.Version;
			IsWindows8OrGreater = (version.Major == 6 && version.Minor >= 2) || version.Major > 6;
		}

		public static string CreateScreenshotFilePath()
		{
			var filePath = Path.Combine(
				Settings.Current.ScreenshotFolder,
				string.Format("KanColle-{0}", DateTimeOffset.Now.LocalDateTime.ToString("yyMMdd-HHmmssff")));

			filePath = Path.ChangeExtension(
				filePath,
				Settings.Current.ScreenshotImageFormat == SupportedImageFormat.Jpeg
					? ".jpg"
					: ".png");

			return filePath;
		}


		/// <summary>
		/// Windows 8 またはそれ以降のバージョンで動作しているかどうかを確認します。
		/// </summary>
		public static bool IsWindows8OrGreater { get; private set; }

		/// <summary>
		/// デザイナーのコンテキストで実行されているかどうかを取得します。
		/// </summary>
		public static bool IsInDesignMode
		{
			get { return DesignerProperties.GetIsInDesignMode(new DependencyObject()); }
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
				if (Registry.GetValue(key, valueName, null) == null)
				{
					Registry.SetValue(key, valueName, Properties.Settings.Default.FeatureBrowserEmulation, RegistryValueKind.DWord);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
