using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(INotifier))]
	public class WindowsNotifier : INotifier
	{
		private readonly INotifier _notifier;

		public WindowsNotifier()
		{
			this._notifier = Windows8Notifier.IsSupported
				? (INotifier) new Windows8Notifier()
				: new Windows7Notifier();
		}

		public void Dispose()
		{
			this._notifier.Dispose();
		}

		public void Initialize()
		{
			this._notifier.Initialize();
		}

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
			this._notifier.Show(type, header, body, activated, failed);
		}
	}
}
