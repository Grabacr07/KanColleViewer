using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
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
		public static IObservable<TResult> TryParse<TResult>(this IObservable<Session> source)
		{
			return source.Select(x => { SvData<TResult> result; return SvData.TryParse(x, out result) ? result : null; })
				.Where(x => x != null && x.IsSuccess)
				.Select(x => x.Data);
		}

		/// <summary>
		/// FiddlerCore でフックした <see cref="Session" /> オブジェクトの <see cref="Session.ResponseBody" /> データを
		/// <see cref="SvData" /> 型にパースします。
		/// </summary>
		public static IObservable<SvData> TryParse(this IObservable<Session> source)
		{
			return source.Select(x =>
			{
				SvData result;
				return SvData.TryParse(x, out result) ? result : null;
			});
		}

		/// <summary>
		/// 例外をキャッチし、<see cref="KanColleClient.Errors" /> プロパティにエラー情報を追加します。
		/// </summary>
		public static IObservable<TSource> OnErrorRetry<TSource>(this IObservable<TSource> source)
		{
			return source.OnErrorRetry((Exception ex) => KanColleClient.Current.Errors.Add(new KanColleError(ex)), TimeSpan.Zero);
		}


		/// <summary>
		/// <see cref="Int32" /> 型の配列に安全にアクセスします。
		/// </summary>
		public static int? Get(this int[] array, int index)
		{
			return array.Length > index ? (int?)array[index] : null;
		}
	}
}
