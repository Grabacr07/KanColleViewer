using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DesktopToast;

namespace Grabacr07.KanColleViewer.Plugins
{
	/// <summary>
	/// Windows 8 のトースト通知機能を提供します。
	/// </summary>
	public class Toast
	{
		#region static members

		/// <summary>
		/// トースト通期機能をサポートしているかどうかを示す値を取得します。
		/// </summary>
		/// <returns>
		/// 動作しているオペレーティング システムが Windows 8 (NT 6.2) 以降の場合は true、それ以外の場合は false。
		/// </returns>
		public static bool IsSupported
		{
			get
			{
				var version = Environment.OSVersion.Version;
				return (version.Major == 6 && version.Minor >= 2) || version.Major > 6;

			}
		}

		#endregion
#if DEBUG
		public const string AppId = "Grabacr07.KanColleViewer.Debug";
#else
		public const string AppId = "Grabacr07.KanColleViewer"; 
#endif

		public event Action Activated;

		public event Action ToastFailed;

		private readonly ToastRequest request;

		public Toast(string header, string body)
		{
			this.request = new ToastRequest
			{
				ToastHeadline = header,
				ToastBody = body,
#if DEBUG
				ShortcutFileName = "提督業も忙しい！ (debug).lnk",
#else
				ShortcutFileName = "提督業も忙しい！.lnk", 
#endif
				ShortcutTargetFilePath = Assembly.GetEntryAssembly().Location,
				AppId = AppId,
			};
		}

		public void Show()
		{
			ToastManager.ShowAsync(this.request)
				.ContinueWith(t =>
				{
					if (t.Result == ToastResult.Activated)
						this.Activated?.Invoke();
					if (t.Result == ToastResult.Failed)
						this.ToastFailed?.Invoke();
				});
		}
	}
}
