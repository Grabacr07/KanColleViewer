using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// サポートされているイメージのファイル形式。
	/// </summary>
	public enum SupportedImageFormat
	{
		/// <summary>
		/// 未指定です。
		/// </summary>
		None,

		/// <summary>
		/// W3C PNG (Portable Network Graphics) イメージ形式。
		/// </summary>
		Png,

		/// <summary>
		/// JPEG (Joint Photographic Experts Group) イメージ形式。
		/// </summary>
		Jpeg,
	}

	public static class SupportedImageFormatExtensions
	{
		public static string ToExtension(this SupportedImageFormat format)
		{
			switch (format)
			{
				case SupportedImageFormat.Png:
					return ".png";

				case SupportedImageFormat.Jpeg:
					return ".jpg";

				default:
					return "";
			}
		}

		public static string ToMimeType(this SupportedImageFormat format)
		{
			switch (format)
			{
				case SupportedImageFormat.Png:
					return "image/png";

				case SupportedImageFormat.Jpeg:
					return "image/jpeg";

				default:
					return "";
			}
		}
	}
}
