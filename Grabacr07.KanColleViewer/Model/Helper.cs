using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Grabacr07.KanColleViewer.Model
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
	}
}
