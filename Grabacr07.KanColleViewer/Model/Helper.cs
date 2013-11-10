using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
				string.Format("KanColle-{0}.png", DateTimeOffset.Now.LocalDateTime.ToString("yyMMdd-HHmmssff")));

			return filePath;
		}


		/// <summary>
		/// Windows 8 またはそれ以降のバージョンで動作しているかどうかを確認します。
		/// </summary>
		public static bool IsWindows8OrGreater { get; private set; }
	}
}
