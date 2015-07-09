using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Grabacr07.KanColleViewer.Models.Data.Xml
{
	/// <summary>
	/// XML ファイルからの読み込みを行う静的メソッドを公開します。
	/// </summary>
	public static class XmlFileReader
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

			FileStream stream = null;			// 読込用ストリーム
			T result = new T();	// 読み込んだデータ

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
			MemoryStream stream = null;			// 読込用ストリーム	
			T result = new T();	// 読み込んだデータ

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
		/// <exception cref="Grabacr07.Utilities.Data.Xml.XmlToolsException">XML ファイルからの読み込みに失敗した場合 (詳細はエラーコードより参照できます)。</exception>
		private static T ReadData<T>(Stream stream) where T : new()
		{
			T result = new T();	// 読み込んだデータ

			// シリアライズ用オブジェクト生成
			var serializer = new XmlSerializer(typeof(T));

			// 与えられたストリームから XML 逆シリアル化
			result = (T)serializer.Deserialize(stream);

			return result;
		}

	}
}
