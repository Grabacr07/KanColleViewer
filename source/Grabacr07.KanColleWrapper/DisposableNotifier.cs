﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatefulModel;
using System.Reactive.Disposables;

namespace Grabacr07.KanColleWrapper
{
	public class DisposableNotifier : Notifier, IDisposable
	{
		protected MultipleDisposable CompositeDisposable { get; }

		public DisposableNotifier()
		{
			this.CompositeDisposable = new MultipleDisposable();
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
