using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Win32;
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


		/// <summary>
		/// 指定した文字列を暗号化します。
		/// </summary>
		/// <param name="source">暗号化する文字列。</param>
		/// <param name="password">暗号化に使用するパスワード。</param>
		/// <returns>暗号化された文字列。</returns>
		public static string EncryptString(string source, string password)
		{
			using (var rijndael = new RijndaelManaged())
			{
				byte[] key, iv;
				GenerateKeyFromPassword(password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
				rijndael.Key = key;
				rijndael.IV = iv;

				using (var encryptor = rijndael.CreateEncryptor())
				{
					var strBytes = Encoding.UTF8.GetBytes(source);
					var encBytes = encryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
					return Convert.ToBase64String(encBytes);
				}
			}
		}

		/// <summary>
		/// 指定された文字列を複合化します。
		/// </summary>
		/// <param name="source">暗号化された文字列。</param>
		/// <param name="password">暗号化に使用したパスワード。</param>
		/// <returns>復号化された文字列。</returns>
		public static string DecryptString(string source, string password)
		{
			try
			{
				using (var rijndael = new RijndaelManaged())
				{
					byte[] key, iv;
					GenerateKeyFromPassword(password, rijndael.KeySize, out key, rijndael.BlockSize, out iv);
					rijndael.Key = key;
					rijndael.IV = iv;

					using (var decryptor = rijndael.CreateDecryptor())
					{
						var strBytes = Convert.FromBase64String(source);
						var decBytes = decryptor.TransformFinalBlock(strBytes, 0, strBytes.Length);
						return Encoding.UTF8.GetString(decBytes);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return null;
			}
		}

		private static void GenerateKeyFromPassword(string password, int keySize, out byte[] key, int blockSize, out byte[] iv)
		{
			var salt = Encoding.UTF8.GetBytes("C98534F6-7286-4BED-83A6-10FD5052ABA6");
			using (var deriveBytes = new Rfc2898DeriveBytes(password, salt) { IterationCount = 1000 })
			{
				key = deriveBytes.GetBytes(keySize / 8);
				iv = deriveBytes.GetBytes(blockSize / 8);
			}
		}
	}
}
