using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Internal;
using Livet;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 1 秒刻みのタイマー機能をサポートする変更通知オブジェクトを表します。
	/// </summary>
	public class TimerNotifier : NotificationObject, IDisposable
	{
		#region static members

		private static readonly IConnectableObservable<long> timer;

		static TimerNotifier()
		{
			timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1)).Publish();
			timer.Connect();
		}

		#endregion

		private readonly IDisposable subscriber;

		public TimerNotifier()
		{
			this.subscriber = timer.Subscribe(_ => this.Tick());
		}

		protected virtual void Tick() { }
		
		public virtual void Dispose()
		{
			this.subscriber.SafeDispose();
		}
	}
}
