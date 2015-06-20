using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper.Models;

namespace Grabacr07.KanColleWrapper
{
	/// <summary>
	/// 整数値の ID をキーとして使用する、艦これマスター データ用のテーブルを定義します。
	/// </summary>
	/// <typeparam name="TValue">マスター データの型。</typeparam>
	public class MasterTable<TValue> : IReadOnlyDictionary<int, TValue> where TValue : class, IIdentifiable
	{
		private readonly IDictionary<int, TValue> dictionary;

		/// <summary>
		/// テーブルから指定した ID の要素を取得します。ID が存在しない場合は null を返します。
		/// </summary>
		public TValue this[int key]
		{
		    get { return this.dictionary.ContainsKey(key) ? this.dictionary[key] : null; }
		}


	    public MasterTable() : this(new List<TValue>()) { }

		public MasterTable(IEnumerable<TValue> source)
		{
			this.dictionary = source.ToDictionary(x => x.Id);
		}

		#region IReadOnlyDictionary<TK, TV> members

		public IEnumerator<KeyValuePair<int, TValue>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int Count
		{
		    get { return this.dictionary.Count; }
		}

	    public bool ContainsKey(int key)
		{
			return this.dictionary.ContainsKey(key);
		}

		public bool TryGetValue(int key, out TValue value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}

		public IEnumerable<int> Keys
		{
		    get { return this.dictionary.Keys; }
		}

	    public IEnumerable<TValue> Values
	    {
	        get { return this.dictionary.Values; }
	    }

	    #endregion
	}
}
