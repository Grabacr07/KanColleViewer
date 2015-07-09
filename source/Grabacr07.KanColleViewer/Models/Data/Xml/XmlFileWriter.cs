using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Grabacr07.KanColleViewer.Models.Data.Xml
{
	/// <summary>
	/// XML ファイルへの書き込みを行う静的メソッドを公開します。
	/// </summary>
	public static class XmlFileWriter
	{
		/// <summary>
		/// 指定した型のデータを XML ファイルに書き込みます。
		/// </summary>
		/// <typeparam name="T">書き込むデータの型。</typeparam>
		/// <param name="saveData">書き込むデータ。</param>
		/// <param name="savePath">保存先のパス。</param>
		public static void WriteXml<T>(this T saveData, string savePath) where T : new()
		{
			// 書き込み先のディレクトリ作成
			var dir = Path.GetDirectoryName(Path.GetFullPath(savePath)) ?? "";
			Directory.CreateDirectory(dir);

			FileStream stream = null;								// 書き込み用ファイルストリーム
			var serializer = new XmlSerializer(typeof(T));	// シリアル化用のオブジェクトを生成

			try
			{
				stream = new FileStream(savePath, FileMode.Create);	// ファイルストリーム生成
				serializer.Serialize(stream, saveData);				// シリアル化して書き込み
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();		// ストリームを閉じる
					stream.Dispose();	// ストリームを解放
				}
			}
		}

	}
}
