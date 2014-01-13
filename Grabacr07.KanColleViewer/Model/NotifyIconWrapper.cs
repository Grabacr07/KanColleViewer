using System;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Notifications;
using Application = System.Windows.Application;

namespace Grabacr07.KanColleViewer.Model
{
	/// <summary>
	/// 通知領域アイコンを利用した通知を提供します。
	/// </summary>
	public class NotifyIconWrapper : INotifyWrapper, IDisposable
	{
		private NotifyIcon notifyIcon;

		public void Initialize()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";
			
			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

			var streamResourceInfo = Application.GetResourceStream(uri);
			if (streamResourceInfo == null) return;

			using (var stream = streamResourceInfo.Stream)
			{
				notifyIcon = new NotifyIcon
				{
					Text = App.ProductInfo.Title,
					Icon = new Icon(stream),
					Visible = true,
				};
			}
		}

		public void Show(
			string header, string body, 
			Action activated, Action<ToastDismissalReason> dismissed = null, Action<Exception> failed = null)
		{
			if (notifyIcon != null)
			{
				notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
			}
		}

		public void Dispose()
		{
			if (notifyIcon != null)
			{
				notifyIcon.Dispose();
			}
		}
	}
}
