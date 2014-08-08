using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(INotifier))]
	[ExportMetadata("Title", "WindowsNotifier")]
	[ExportMetadata("Description", "Windows OS の機能 (トースト通知・バルーン通知) を使用して通知します。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class WindowsNotifier : INotifier
	{
		private readonly INotifier notifier;

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
			this.notifier.Show(type, header, body, activated, failed);
		}

		public object GetSettingsView()
		{
			return this.notifier.GetSettingsView();
		}
	}
}
