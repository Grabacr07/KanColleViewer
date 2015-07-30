using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Grabacr07.KanColleViewer.Models.Migration
{
	/// <summary>互換性のために残されています。</summary>
	[Obsolete]
	[EditorBrowsable(EditorBrowsableState.Never)]
	// ReSharper disable once InconsistentNaming
	public static class _XmlFileReader
	{
		/// <summary>
		/// 指定した型のデータを XML ファイルから読み込みます。
		/// </summary>
		/// <typeparam name="T">読み込むデータの型。</typeparam>
		/// <param name="filePath">読み込むデータとなる XML ファイルのパス。</param>
		/// <returns>読み込んだデータ。</returns>
		public static T ReadXml<T>(this string filePath) where T : new()
		{
			if (filePath == null || !File.Exists(filePath))
			{
				// 指定されたファイルパスの XML ファイルが見つからない場合
				throw new FileNotFoundException("ファイルが見つかりません。", filePath);
			}

			FileStream stream = null; // 読込用ストリーム
			T result; // 読み込んだデータ

			try
			{
				// ファイルストリームを生成し、XML ファイルを読み込み
				stream = new FileStream(filePath, FileMode.Open);
				result = ReadData<T>(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}
			}

			return result;
		}


		/// <summary>
		/// 指定した型のデータを XML ファイルから読み込みます。
		/// </summary>
		/// <typeparam name="T">読み込むデータの型。</typeparam>
		/// <param name="data">読み込むデータとなる XML ファイルのバイト配列。</param>
		/// <returns>読み込んだデータ。</returns>
		public static T ReadData<T>(byte[] data) where T : new()
		{
			MemoryStream stream = null; // 読込用ストリーム	
			T result; // 読み込んだデータ

			try
			{
				// メモリストリームを生成し、バイトデータを読み込み
				stream = new MemoryStream(data);
				result = ReadData<T>(stream);
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
					stream.Dispose();
				}
			}

			return result;
		}


		/// <summary>
		/// 指定した型のデータを XML ファイルから読み込みます。
		/// </summary>
		/// <typeparam name="T">読み込むデータの型。</typeparam>
		/// <param name="stream">読込先となる XML ファイルのストリーム。</param>
		/// <returns>読み込んだデータ。</returns>
		private static T ReadData<T>(Stream stream) where T : new()
		{
			var type = typeof(T);
			var attr = Attribute.GetCustomAttribute(type, typeof(XmlRootAttribute));
			// シリアライズ用オブジェクト生成
			var serializer = attr == null 
				? new XmlSerializer(type)
				: new XmlSerializer(type, (XmlRootAttribute)attr);

			// 与えられたストリームから XML 逆シリアル化
			var result = (T)serializer.Deserialize(stream);

			return result;
		}
	}
}
