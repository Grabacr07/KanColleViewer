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

		public void Notify(INotification notify)
		{
			foreach (var x in this.notifiers)
			{
				x.Notify(notify);
			}
		}
	}
}
