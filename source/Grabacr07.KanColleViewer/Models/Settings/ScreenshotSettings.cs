using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	/// <summary>
	/// スクリーンショットに関連する設定を表す静的プロパティを公開します。
	/// </summary>
	public static class ScreenshotSettings
	{
		/// <summary>
		/// スクリーンショットの保存先フォルダーを表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<string> Destination { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Local, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

		/// <summary>
		/// スクリーンショットのファイル名を表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<string> Filename { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Local, "KanColle-{0:d04}.png");

		/// <summary>
		/// スクリーンショットのイメージ形式を表す設定値を取得します。
		/// </summary>
		public static SerializableProperty<SupportedImageFormat> Format { get; }
			= new SerializableProperty<SupportedImageFormat>(GetKey(), Providers.Local, SupportedImageFormat.Png);


		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(ScreenshotSettings) + "." + propertyName;
		}
	}
}
