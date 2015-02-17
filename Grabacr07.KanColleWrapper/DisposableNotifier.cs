using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	public class DisposableNotifier : NotificationObject, IDisposable
	{
		protected LivetCompositeDisposable CompositeDisposable { get; private set; }

		public DisposableNotifier()
		{
			this.CompositeDisposable = new LivetCompositeDisposable();
		}

		public void Dispose()
		{
			this.Dispose(true);
			this.CompositeDisposable.Dispose();

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) { }
	}
}
