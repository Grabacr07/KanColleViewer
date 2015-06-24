using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[InheritedExport(typeof(INotifier))]
	[ExportMetadata("Guid", "6EDE38C8-412D-4A73-8FE3-A9D20EB9F0D2")]
	public abstract class NotifierBase : INotifier, IDisposable
	{
		public bool Initialized { get; private set; }

		public abstract bool IsSupported { get; }

		protected abstract void InitializeCore();

		protected abstract void ShowCore(NotifyType type, string header, string body, Action activated, Action<Exception> failed);

		public void Initialize()
		{
			if (this.Initialized) return;

			this.InitializeCore();
			this.Initialized = true;
		}

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
			if (!this.IsSupported) return;

			this.Initialize();
			this.ShowCore(type, header, body, activated, failed);
		}

		public virtual void Dispose() { }
	}
}
