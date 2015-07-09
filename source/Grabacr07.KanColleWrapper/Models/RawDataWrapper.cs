using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Livet;

namespace Grabacr07.KanColleWrapper.Models
{
	/// <summary>
	/// 艦これ API からの応答に含まれる api_data (生のデータ) をラップします。
	/// </summary>
	/// <typeparam name="T">ラップする api_data (生のデータ) の型。</typeparam>
	public abstract class RawDataWrapper<T> : NotificationObject
	{
		/// <summary>
		/// 艦これ API からの応答に含まれる api_data をパースした生のデータを取得します。
		/// </summary>
		internal T RawData { get; private set; }

		/// <summary>
		/// ラップする api_data (生のデータ) を使用して、<see cref="RawDataWrapper{T}" /> クラスの新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="rawData">ラップする生のデータ。</param>
		protected RawDataWrapper(T rawData)
		{
			this.RawData = rawData;
		}

		/// <summary>
		/// Update the RawData for all derived classes.
		/// </summary>
		/// <param name="rawData">RawData to update to.</param>
		protected void UpdateRawData(T rawData)
		{
			this.RawData = rawData;
		}
	}
}
