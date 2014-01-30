using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
