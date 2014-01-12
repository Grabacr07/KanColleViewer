using System;
using System.Drawing;
using System.Windows.Forms;

namespace Grabacr07.KanColleViewer.Model
{
	/// <summary>
	/// 通知領域アイコンを利用した通知を提供します。
	/// </summary>
	public static class NotifyIconWrapper
	{
		private static NotifyIcon notifyIcon;

		public static void Initialize()
		{
			var icon = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";
			Uri uri = null;

			if (Uri.TryCreate(icon, UriKind.Absolute, out uri))
			{
				using (var stream = App.GetResourceStream(uri).Stream)
				{
					notifyIcon = new NotifyIcon
					{
						Text = App.ProductInfo.Title,
						Icon = new Icon(stream),
						Visible = true,
					};
				}
			}
		}

		public static void Show(string title, string text)
		{
			if (notifyIcon != null)
			{
				notifyIcon.ShowBalloonTip(1000, title, text, ToolTipIcon.None);
			}
		}

		public static void Dispose()
		{
			if (notifyIcon != null)
			{
				notifyIcon.Dispose();
			}
		}
	}
}
