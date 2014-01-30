using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Internal;

namespace Grabacr07.KanColleViewer.Models
{
	/// <summary>
	/// Windows の通知機能を提供します。Windows のバージョンに依存する実装の差異は隠蔽されます。
	/// </summary>
	public static class WindowsNotification
	{
		private static IWindowsNotifier notifier;

		/// <summary>
		/// 現在の OS バージョンに適合する通知機能へアクセスできるようにします。
		/// </summary>
		public static IWindowsNotifier Notifier
		{
			get { return notifier ?? (notifier = Windows8Notifier.IsSupported ? (IWindowsNotifier)new Windows8Notifier() : new Windows7Notifier()); }
		}
	}
}
