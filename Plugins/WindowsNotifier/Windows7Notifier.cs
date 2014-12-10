using Grabacr07.KanColleViewer.Composition;
using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class Windows7Notifier : INotifier
	{
		CustomSound sound = new CustomSound();
		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;

		public void Initialize()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
				return;

			var streamResourceInfo = Application.GetResourceStream(uri);
			if (streamResourceInfo == null)
				return;

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

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
			if (this.notifyIcon == null)
				return;

			if (activated != null)
			{
				this.notifyIcon.BalloonTipClicked -= this.activatedAction;

				this.activatedAction = (sender, args) => activated();
				this.notifyIcon.BalloonTipClicked += this.activatedAction;
			}
			sound.SoundOutput(header, false);
			notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
		}

		public object GetSettingsView()
		{
			return null;
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
