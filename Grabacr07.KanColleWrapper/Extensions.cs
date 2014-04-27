using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	public static class Extensions
	{
		/// <summary>
		/// コレクションを展開し、メンバーの文字列表現を指定した区切り文字で連結した文字列を返します。
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
		public static bool HasItems<T>(this IEnumerable<T> source)
		{
			return source != null && source.Any();
		}
	}
}
