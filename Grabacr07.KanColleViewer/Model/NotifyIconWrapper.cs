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
		private static NotifyIcon _notifyIcon;
		private NotifyIconWrapper() { }

		public static void Initialize()
		{
			_notifyIcon = new NotifyIcon();
			_notifyIcon.Text = "KanColleViewer";
			var uri = new Uri("pack://application:,,,/KanColleViewer;Component/Assets/app.ico");
			var stream = System.Windows.Application.GetResourceStream(uri).Stream;
			_notifyIcon.Icon = new Icon(stream);
			_notifyIcon.Visible = true;
		}

		public static void Show(string title, string text)
		{
			_notifyIcon.ShowBalloonTip(1000, title, text, ToolTipIcon.None);
		}

		public static void Dispose()
		{
			_notifyIcon.Dispose();
		}
	}
}
