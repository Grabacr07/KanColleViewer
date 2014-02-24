using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Win32;
using MetroRadiance.Core;
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


		/// <summary>
		/// キャッシュを削除します。
		/// </summary>
		/// <seealso cref="http://support.microsoft.com/kb/326201/ja"/>
		public static Task<bool> DeleteInternetCache()
		{
			return Task.Factory.StartNew(() => DeleteInternetCacheCore());
		}

		private static bool DeleteInternetCacheCore()
		{
			// ReSharper disable InconsistentNaming
			const int CACHEGROUP_SEARCH_ALL = 0x0;
			const int ERROR_NO_MORE_ITEMS = 259;
			const uint CacheEntryType_Cookie = 1048577;
			const uint CacheEntryType_History = 2097153;
			// ReSharper restore InconsistentNaming

			long groupId = 0;
			var cacheEntryInfoBufferSizeInitial = 0;

			var enumHandle = WinInet.FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
			if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error()) return false;

			enumHandle = WinInet.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
			if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error()) return false;

			var cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
			var cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
			enumHandle = WinInet.FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

			while (true)
			{
				var internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(
					cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));
				cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;

				var type = internetCacheEntry.CacheEntryType;
				var result = false;

				if (type != CacheEntryType_Cookie && type != CacheEntryType_History)
				{
					result = WinInet.DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
				}

				if (!result)
				{
					result = WinInet.FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
				}
				if (!result && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
				{
					break;
				}
				if (!result && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
				{
					cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
					cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr)cacheEntryInfoBufferSize);
					WinInet.FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
				}
			}

			Marshal.FreeHGlobal(cacheEntryInfoBuffer);

			return true;
		}
	}
}
