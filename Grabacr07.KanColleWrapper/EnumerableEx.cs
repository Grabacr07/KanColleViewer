using System;
using System.Collections.Generic;

namespace Grabacr07.KanColleWrapper
{
	public static class EnumerableEx
	{
		public static IEnumerable<TResult> Return<TResult>(TResult value)
		{
			yield return value;
		}

		public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var set = new HashSet<TKey>(EqualityComparer<TKey>.Default);

			foreach (var item in source)
			{
				var key = keySelector(item);
				if (set.Add(key)) yield return item;
			}
		}
	}
}
