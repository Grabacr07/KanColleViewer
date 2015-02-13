using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grabacr07.KanColleViewer.Composition;
using Application = System.Windows.Application;
using Grabacr07.KanColleViewer;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace Grabacr07.KanColleViewer.Plugins
{
    internal class Windows7Notifier : INotifier
    {
        private MediaPlayer mp;
        private NotifyIcon notifyIcon;
        private EventHandler activatedAction;

        public void Initialize()
        {
            mp = new MediaPlayer();
            const string iconUri = "pack://application:,,,/KanColleViewer;Component/Assets/app.ico";

            Uri uri;
            if (!Uri.TryCreate(iconUri, UriKind.Absolute, out uri))
                return;

            var streamResourceInfo = Application.GetResourceStream(uri);
            if (streamResourceInfo == null)
                return;

            if (!Directory.Exists("Sounds"))
            {
                Directory.CreateDirectory("Sounds");
                DirectoryInfo d = new DirectoryInfo("Sounds");
                d.CreateSubdirectory(NotifyType.Build.ToString());
                d.CreateSubdirectory(NotifyType.Expedition.ToString());
                d.CreateSubdirectory(NotifyType.Rejuvenated.ToString());
                d.CreateSubdirectory(NotifyType.Repair.ToString());
            }

            using (var stream = streamResourceInfo.Stream)
            {
                this.notifyIcon = new NotifyIcon
                {
                    Text = App.ProductInfo.Title,
                    Icon = new Icon(stream),
                };
                ContextMenu menu = new ContextMenu();

                MenuItem closeItem = new MenuItem();
                closeItem.Text = "KanColleViewerを終了";
                closeItem.Click += new EventHandler(delegate
                {
                    Process[] killprocess = Process.GetProcessesByName("KanColleViewer");
                    foreach (Process p in killprocess)
                    {
                        p.Kill();
                    }
                });
                MenuItem addItem = new MenuItem();
                addItem.Text = "シャットダウン";
                addItem.Click += new EventHandler(delegate { Process.Start("shutdown.exe", "-s -t 00"); });
                menu.MenuItems.Add(addItem);
                menu.MenuItems.Add(closeItem);
                notifyIcon.ContextMenu = menu;
            }
        }

        public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
        {
            if (this.notifyIcon == null)
                return;

            try
            {
                mp.Dispatcher.Invoke(new Action(() =>
                {
                    var Audiofile = GetRandomSound(type.ToString());
                    mp.Close();
                    mp.Open(new Uri(Audiofile));
                    mp.Play();
                }));
            }
            catch { }

            if (activated != null)
            {
                this.notifyIcon.BalloonTipClicked -= this.activatedAction;
                this.activatedAction = (sender, args) => activated();
                this.notifyIcon.BalloonTipClicked += this.activatedAction;
            }
            notifyIcon.ShowBalloonTip(2000, header, body, ToolTipIcon.Info);

        }

        public string GetRandomSound(string type)
        {
            try
            {
                if (!Directory.Exists("Sounds\\"))
                {
                    Directory.CreateDirectory("Sounds");
                }

                if (!Directory.Exists("Sounds\\" + type))
                {
                    Directory.CreateDirectory("Sounds\\" + type);
                    return null;
                }

                List<string> FileList = Directory.GetFiles("Sounds\\" + type, "*.wav", SearchOption.AllDirectories)
                    .Concat(Directory.GetFiles("Sounds\\" + type, "*.mp3", SearchOption.AllDirectories)).ToList();

                if (FileList.Count > 0)
                {
                    Random Rnd = new Random();
                    return FileList[Rnd.Next(0, FileList.Count)];
                }
            }
            catch { }
            return null;
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
                this.mp.Close();
                this.mp = null;
            }
        }
    }
}
