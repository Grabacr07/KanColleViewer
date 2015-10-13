using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grabacr07.KanColleViewer.Models;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class BalloonNotifier : NotifierBase
	{
		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;
		private CustomSound sound = new CustomSound();

		public override bool IsSupported => !Toast.IsSupported;

		protected override void InitializeCore()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
				return;

			var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
			if (streamResourceInfo == null)
				return;

			using (var stream = streamResourceInfo.Stream)
			{
				this.notifyIcon = new NotifyIcon
				{
					Text = ProductInfo.Title,
					Icon = new Icon(stream),
					Visible = true,
				};
			}
		}

		protected override void NotifyCore(string header, string body, Action activated, Action<Exception> failed)
		{
			if (this.notifyIcon == null) return;

			if (activated != null)
			{
				this.notifyIcon.BalloonTipClicked -= this.activatedAction;

				this.activatedAction = (sender, args) => activated();
				this.notifyIcon.BalloonTipClicked += this.activatedAction;
			}

			this.sound.SoundOutput(header, false);

			this.notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
		}

		public override void Dispose()
		{
			this.notifyIcon?.Dispose();
		}
	}
}
