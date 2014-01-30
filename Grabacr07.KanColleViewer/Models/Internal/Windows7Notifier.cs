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
	internal class Windows7Notifier : IWindowsNotifier
	{
		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;

		public void Initialize()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri)) return;

			var streamResourceInfo = Application.GetResourceStream(uri);
			if (streamResourceInfo == null) return;

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

		public void Show(string header, string body, Action activated, Action<Exception> failed = null)
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

		public void Dispose()
		{
			if (this.notifyIcon != null)
			{
				this.notifyIcon.Dispose();
			}
		}
	}
}
