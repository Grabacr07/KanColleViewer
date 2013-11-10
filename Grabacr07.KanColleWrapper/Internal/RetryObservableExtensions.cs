using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Internal
{
	public static class RetryObservableExtensions
	{
		/// <summary>
		/// When catched exception, do onError action and repeat observable sequence.
		/// </summary>
		public static IObservable<TSource> OnErrorRetry<TSource, TException>(
			this IObservable<TSource> source, Action<TException> onError)
			where TException : Exception
		{
			return source.OnErrorRetry(onError, TimeSpan.Zero);
		}

		/// <summary>
		/// When catched exception, do onError action and repeat observable sequence after delay time.
		/// </summary>
		public static IObservable<TSource> OnErrorRetry<TSource, TException>(
			this IObservable<TSource> source, Action<TException> onError, TimeSpan delay)
			where TException : Exception
		{
			return source.OnErrorRetry(onError, int.MaxValue, delay);
		}

		/// <summary>
		/// When catched exception, do onError action and repeat observable sequence during within retryCount.
		/// </summary>
		public static IObservable<TSource> OnErrorRetry<TSource, TException>(
			this IObservable<TSource> source, Action<TException> onError, int retryCount)
			where TException : Exception
		{
			return source.OnErrorRetry(onError, retryCount, TimeSpan.Zero);
		}

		/// <summary>
		/// When catched exception, do onError action and repeat observable sequence after delay time during within retryCount.
		/// </summary>
		public static IObservable<TSource> OnErrorRetry<TSource, TException>(
			this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay)
			where TException : Exception
		{
			return source.OnErrorRetry(onError, retryCount, delay, Scheduler.Default);
		}

		/// <summary>
		/// When catched exception, do onError action and repeat observable sequence after delay time(work on delayScheduler) during within retryCount.
		/// </summary>
		public static IObservable<TSource> OnErrorRetry<TSource, TException>(
			this IObservable<TSource> source, Action<TException> onError, int retryCount, TimeSpan delay, IScheduler delayScheduler)
			where TException : Exception
		{
			var result = Observable.Defer(() =>
			{
				var dueTime = (delay.Ticks < 0) ? TimeSpan.Zero : delay;
				var empty = Observable.Empty<TSource>();
				var count = 0;

				IObservable<TSource> self = null;
				self = source.Catch((TException ex) =>
				{
					onError(ex);

					return (++count < retryCount)
						? (dueTime == TimeSpan.Zero)
							? self.SubscribeOn(Scheduler.CurrentThread)
							: empty.Delay(dueTime, delayScheduler).Concat(self).SubscribeOn(Scheduler.CurrentThread)
						: Observable.Throw<TSource>(ex);
				});
				return self;
			});

			return result;
		}
	}
}
