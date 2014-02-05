using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Media;
using System.IO;
using Grabacr07.KanColleViewer.Models;
using Application = System.Windows.Application;


namespace Grabacr07.KanColleViewer.Models.Internal{
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
            //소리재생 해봅시다 한번
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            string Main_folder = Path.GetDirectoryName(location);
            SoundPlayer notify_sound = new SoundPlayer(Main_folder + "\\notify.wav");
            notify_sound.Load();
            var checkV=Volume.GetInstance();

            if (checkV.IsMute==false && System.IO.File.Exists(Main_folder + "\\notify.wav") == true)
            {
                notify_sound.Play();                
			}
            
            else
            {
                System.Media.SystemSounds.Beep.Play();                
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
