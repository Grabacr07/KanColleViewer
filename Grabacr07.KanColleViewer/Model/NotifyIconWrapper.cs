using System;
using System.Drawing;
using System.Windows.Forms;

namespace Grabacr07.KanColleViewer.Model
{
	/// <summary>
	/// 通知領域アイコンを利用した通知を提供します。
	/// </summary>
	public class NotifyIconWrapper
	{
		private static NotifyIcon notifyIcon;
		private NotifyIconWrapper() { }

		public static void Initialize()
		{
			notifyIcon = new NotifyIcon();
			notifyIcon.Text = "KanColleViewer";
			var uri = new Uri("pack://application:,,,/KanColleViewer;Component/Assets/app.ico");
			var stream = System.Windows.Application.GetResourceStream(uri).Stream;
			notifyIcon.Icon = new Icon(stream);
			notifyIcon.Visible = true;
		}

		public static void Show(string title, string text)
		{
			notifyIcon.ShowBalloonTip(1000, title, text, ToolTipIcon.None);
		}

		public static void Dispose()
		{
			notifyIcon.Dispose();
		}
	}
}
