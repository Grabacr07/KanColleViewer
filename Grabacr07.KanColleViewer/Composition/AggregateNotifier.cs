using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	public class AggregateNotifier : INotifier
	{
		public readonly INotifier[] _notifiers;

		public AggregateNotifier(IEnumerable<INotifier> notifiers)
		{
			this._notifiers = notifiers.ToArray();
		}

		public void Dispose()
		{
			this._notifiers.ForEach(x => x.Dispose());
		}

		public void Initialize()
		{
			this._notifiers.ForEach(x => x.Initialize());
		}

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
			this._notifiers.ForEach(x => x.Show(type, header, body, activated, failed));
		}
	}
}
