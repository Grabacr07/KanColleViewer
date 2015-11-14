using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.Plugins
{
	[InheritedExport(typeof(INotifier))]
	[ExportMetadata("Guid", "E1F5EF36-18F5-42D5-896F-9476CD2F3128")]
	public abstract class AudibleNotifierBase : INotifier, IDisposable
	{
		public bool Initialized { get; private set; }

		public abstract bool IsSupported { get; }

		protected abstract void InitializeCore();

		protected abstract void NotifyCore(string type);

		public void Initialize()
		{
			if (this.Initialized) return;

			this.InitializeCore();
			this.Initialized = true;
		}

		public void Notify(INotification notification)
		{
			if (!this.IsSupported) return;

			this.Initialize();
			this.NotifyCore(notification.Type);
		}

		public virtual void Dispose() { }
	}
}
