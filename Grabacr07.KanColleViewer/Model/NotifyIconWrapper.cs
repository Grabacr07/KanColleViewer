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
			_notifyIcon.Icon = KanColleViewer.Resource1.Icon1;
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
