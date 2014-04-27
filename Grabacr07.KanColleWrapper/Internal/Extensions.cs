using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Fiddler;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper.Internal
{
	internal static class Extensions
	{
		public static string GetResponseAsJson(this Session session)
		{
			return session.GetResponseBodyAsString().Replace("svdata=", "");
		}

		public static void SafeDispose(this IDisposable resource)
		{
			if (resource != null) resource.Dispose();
		}

		/// <summary>
		/// FiddlerCore でフックした <see cref="Session"/> オブジェクトの <see cref="Session.ResponseBody"/> データを
		/// <typeparamref name="TResult"/> 型にパースします。
		/// </summary>
		public static IObservable<SvData<TResult>> TryParse<TResult>(this IObservable<Session> source)
		{
			Func<Session, SvData<TResult>> converter = session =>
			{
				SvData<TResult> result;
				return SvData.TryParse(session, out result) ? result : null;
			};

			return source.Select(converter).Where(x => x != null && x.IsSuccess);
		}

		/// <summary>
		/// FiddlerCore でフックした <see cref="Session" /> オブジェクトの <see cref="Session.ResponseBody" /> データを
		/// <see cref="SvData" /> 型にパースします。
		/// </summary>
		public static IObservable<SvData> TryParse(this IObservable<Session> source)
		{
			Func<Session, SvData> converter = session =>
			{
				SvData result;
				return SvData.TryParse(session, out result) ? result : null;
			};

			return source.Select(converter).Where(x => x != null && x.IsSuccess);
		}

		/// <summary>
		/// <see cref="Int32" /> 型の配列に安全にアクセスします。
		/// </summary>
		public static int? Get(this int[] array, int index)
		{
			return array.Length > index ? (int?)array[index] : null;
		}

		public static Task WhenAll(this IEnumerable<Task> tasks)
		{
			return Task.WhenAll(tasks);
		}

		public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
		{
			return Task.WhenAll(tasks);
		}
	}
}
