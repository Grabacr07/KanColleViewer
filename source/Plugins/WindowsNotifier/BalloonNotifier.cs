using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Plugins.Properties;

namespace Grabacr07.KanColleViewer.Plugins
{
	internal class BalloonNotifier : NotifierBase
	{
		private static readonly Settings settings = Settings.Default;

		private NotifyIcon notifyIcon;
		private EventHandler activatedAction;
		private CustomSound sound = new CustomSound();

		public override bool IsSupported => !Toast.IsSupported;

        // For modernNotify
        private Image icon = null;
        private ModernNotify.ModernNotify modernNotify;

		protected override void InitializeCore()
		{
			const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

			Uri uri;
			if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
				return;

			var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
			if (streamResourceInfo == null)
				return;

            modernNotify = ModernNotify.ModernNotify.Instance;

			System.Windows.Application.Current.Dispatcher.Invoke(() =>
			{
				using (var stream = streamResourceInfo.Stream)
				{
					this.notifyIcon = new NotifyIcon
					{
						Text = ProductInfo.Title,
						Icon = new Icon(stream),
						Visible = true,
					};
                    icon = this.notifyIcon.Icon.ToBitmap();
				}
			});
		}

		protected override void NotifyCore(string header, string body, Action activated, Action<Exception> failed)
		{
            if (settings.UseModern)
            {
                modernNotify.Notify(new ModernNotify.NotifyData()
                {
                    Title = header,
                    Content = body,
                    Icon = icon,
                    Activated = activated,
                    Failed = failed
                });
            }
            else
            {
                if (this.notifyIcon == null) return;

                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    if (activated != null)
                    {
                        this.notifyIcon.BalloonTipClicked -= this.activatedAction;

                        this.activatedAction = (sender, args) => activated();
                        this.notifyIcon.BalloonTipClicked += this.activatedAction;
                    }

                    this.notifyIcon.ShowBalloonTip(1000, header, body, ToolTipIcon.None);
                });
                this.sound.SoundOutput(header, false);
            }
		}

		public override void Dispose()
		{
			this.notifyIcon?.Dispose();
		}
	}
}
