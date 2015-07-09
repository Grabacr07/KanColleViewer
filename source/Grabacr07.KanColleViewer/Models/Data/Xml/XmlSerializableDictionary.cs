using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Grabacr07.KanColleViewer.Models.Data.Xml
{
	/// <summary>
	/// XML シリアライズ可能なキーと値のコレクションを表します。
	/// </summary>
	/// <typeparam name="TKey">ディクショナリ内のキーの型。</typeparam>
	/// <typeparam name="TValue">ディクショナリ内の値の型。</typeparam>
	public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
	{
		/// <summary>
		/// 空で、規定の初期量を備え、キーの型の規定の等値比較子を使用する、<see cref="T:Grabacr07.KanColleViewer.Models.Data.Xml.XmlSerializableDictionary`2"/>
		/// クラスの新しいインスタンスを初期化します。
		/// </summary>
		public XmlSerializableDictionary() : base() { }

		public XmlSerializableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
			: base(dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) { }

		public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary)
			: base(dictionary) { }

		/// <summary>
		/// キーと値のコレクションの XML 表現からキーと値のコレクションを生成します。
		/// </summary>
		/// <param name="reader">オブジェクトの逆シリアル化元である <see cref="T:System.Xml.XmlReader"/> ストリーム。</param>
		public void ReadXml(XmlReader reader)
		{
			var serializer = new XmlSerializer(typeof(LocalKeyValuePair));

			reader.Read();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				var lkvp = serializer.Deserialize(reader) as LocalKeyValuePair;
				if (lkvp != null) base.Add(lkvp.Key, lkvp.Value);
			}
			reader.Read();
		}

		/// <summary>
		/// キーと値のコレクションを XML 表現に変換します。
		/// </summary>
		/// <param name="writer">オブジェクトのシリアル化先の <see cref="T:System.Xml.XmlWriter"/> ストリーム。</param>
		public void WriteXml(XmlWriter writer)
		{
			var serializer = new XmlSerializer(typeof(LocalKeyValuePair));

			foreach (var key in this.Keys)	// 書き込み
			{
				serializer.Serialize(writer, new LocalKeyValuePair(key, this[key]));
			}
		}

		/// <summary>
		/// ディクショナリ内のキーと値の型を使用した、キーと値のペアを表します。
		/// </summary>
		public class LocalKeyValuePair
		{
			/// <summary>
			/// キーを取得または設定します。
			/// </summary>
			public TKey Key { get; set; }

			/// <summary>
			/// キーに対応する値を取得または設定します。
			/// </summary>
			public TValue Value { get; set; }

			/// <summary>
			/// <see cref="T:Grabacr07.Utilities.Data.Xml.XmlSerializableDictionary.LocalKeyValuePair"/>
			/// クラスの新しいインスタンスを初期化します。
			/// </summary>
			public LocalKeyValuePair()
			{
			}

			/// <summary>
			/// キーと値のペアを指定し、<see cref="T:Grabacr07.Utilities.Data.Xml.XmlSerializableDictionary.LocalKeyValuePair"/>
			/// クラスの新しいインスタンスを初期化します。
			/// </summary>
			/// <param name="key">キー。</param>
			/// <param name="value">キーに対応する値。</param>
			public LocalKeyValuePair(TKey key, TValue value)
			{
				this.Key = key;
				this.Value = value;
			}
		}


		/// <summary>
		/// 現在のインスタンスと同じ値の新しいインスタンスを生成します。
		/// </summary>
		public object Clone()
		{
			return this.CloneNative();
		}

		/// <summary>
		/// 現在のインスタンスと同じ値の、厳密に型指定された新しいインスタンスを生成します。
		/// </summary>
		public XmlSerializableDictionary<TKey, TValue> CloneNative()
		{
			var result = new XmlSerializableDictionary<TKey, TValue>();

			foreach (var key in this.Keys)
			{
				result.Add(key, this[key]);
			}

			return result;
		}



		/// <summary>
		/// スキーマ定義を返します。
		/// </summary>
		/// <remarks>
		/// 常に null が帰ります。
		/// </remarks>
		/// <returns>WriteXml メソッドによって生成され ReadXml メソッドによって処理される、オブジェクトの XML 表現を記述する XmlSchema。</returns>
		public XmlSchema GetSchema()
		{
			return null;    // スキーマの定義は面倒なので省略
		}
	}
}
