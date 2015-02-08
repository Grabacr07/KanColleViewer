using Grabacr07.KanColleViewer.Composition;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(INotifier))]
	[ExportMetadata("Title", "WindowsNotifier")]
	[ExportMetadata("Description", "기존 플러그인의 개조 버전입니다. 토스트,툴팁 알림에 원하는 소리를 설정할 수 있습니다.")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07,@Freyya312")]

	public class WindowsNotifier : INotifier
	{
		private INotifier notifier;

		public WindowsNotifier()
		{
			this.notifier = Windows8Notifier.IsSupported
				? (INotifier) new Windows8Notifier()
				: new Windows7Notifier();
		}

		public void Dispose()
		{
			this.notifier.Dispose();
		}

		public void Initialize()
		{
			this.notifier.Initialize();
		}

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
			try//임시적 조치
			{
				this.notifier.Show(type, header, body, activated, failed);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
				try
				{
					this.notifier = new Windows7Notifier();
					this.notifier.Show(type, header, body, activated, failed);
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex);
				}
				
			}
		}

		public object GetSettingsView()
		{
			return this.notifier.GetSettingsView();
		}
	}
}
