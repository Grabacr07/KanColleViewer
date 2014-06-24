using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Composition
{
	/// <summary>
	/// 複数の <see cref="INotifier"/> を集約した通知機能を提供します。
	/// </summary>
	public class AggregateNotifier : INotifier
	{
		private readonly INotifier[] notifiers;

		public AggregateNotifier(IEnumerable<INotifier> notifiers)
		{
			this.notifiers = notifiers.ToArray();
		}

		public void Dispose()
		{
			this.notifiers.ForEach(x => x.Dispose());
		}

		public void Initialize()
		{
			this.notifiers.ForEach(x => x.Initialize());
		}

		public void Show(NotifyType type, string header, string body, Action activated, Action<Exception> failed = null)
		{
			this.notifiers.ForEach(x => x.Show(type, header, body, activated, failed));
		}

		public object GetSettingsView()
		{
			return null;
		}
	}
}
