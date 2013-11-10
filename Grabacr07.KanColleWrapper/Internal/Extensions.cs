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
			return array.Length <= index + 1 ? (int?)array[index] : null;
		}

		/// <summary>
		/// コレクションを展開し、メンバーの文字列表現を指定したセパレーターで連結した文字列を返します。
		/// </summary>
		/// <typeparam name="T">コレクションに含まれる任意の型。</typeparam>
		/// <param name="source">対象のコレクション。</param>
		/// <param name="separator">セパレーターとして使用する文字列。</param>
		/// <returns>コレクションの文字列表現を展開し、指定したセパレーターで連結した文字列。</returns>
		public static string ToString<T>(this IEnumerable<T> source, string separator)
		{
			return string.Join(separator, source);
		}

		/// <summary>
		/// シーケンスが null でなく、1 つ以上の要素を含んでいるかどうかを確認します。
		/// </summary>
		public static bool HasValue<T>(this IEnumerable<T> source)
		{
			return source != null && source.Any();
		}
	}
}
