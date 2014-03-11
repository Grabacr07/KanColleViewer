using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Grabacr07.KanColleViewer.Models.Internal
{
	/// <summary>
	/// 通知領域アイコンを利用した通知機能を提供します。
	/// </summary>
	internal class Windows7Notifier : WindowsNotifier
	{
		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;

		public override void abstractInit()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
			{
				throw new System.InvalidOperationException
				(
					String.Format
					(
						"Windows7Notifier Uri create failed! UriString: {0}, UriKind: {1}", iconUri.ToString(), UriKind.Absolute.ToString()
					)
				);
			}

			var streamResourceInfo = Application.GetResourceStream(uri);
			if (streamResourceInfo == null) throw new System.InvalidOperationException("Windows7Notifier Application.GetResourceStream returned null data");

			using (var stream = streamResourceInfo.Stream)
			{
				this.notifyIcon = new NotifyIcon
				{
					Text = App.ProductInfo.Title,
					Icon = new Icon(stream),
					Visible = true,
				};
			}
		}

		public override void Show(string header, string body, Action activated, Action<Exception> failed = null)
		{
			if (this.notifyIcon == null) return;

			if (activated != null)
			{
				this.notifyIcon.BalloonTipClicked -= this.activatedAction;

				this.activatedAction = (sender, args) => activated();
				this.notifyIcon.BalloonTipClicked += this.activatedAction;
			}

			notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
		}

		public override void Dispose()
		{
			if (this.notifyIcon != null)
			{
				this.notifyIcon.Dispose();
			}
		}
	}
}
