using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public static class GeneralSettings
	{
		/// <summary>
		/// カルチャ設定を取得します。
		/// </summary>
		public static SerializableProperty<string> Culture { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming) { AutoSave = true };

		/// <summary>
		/// アプリケーションがプロキシ モード (ブラウザーを表示せず、プロキシとしてのみ使用するモード) で動作するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> IsProxyMode { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Roaming, false);

		/// <summary>
		/// アプリケーション終了時の確認動作を示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<ExitConfirmationType> ExitConfirmationType { get; }
			= new SerializableProperty<ExitConfirmationType>(GetKey(), Providers.Roaming, Models.ExitConfirmationType.None) { AutoSave = true };

		/// <summary>
		/// ブラウザーの拡大鏡を示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<double> BrowserZoomFactor { get; }
			= new SerializableProperty<double>(GetKey(), Providers.Local, 1.0);

		/// <summary>
		/// ユーザー スタイル シート設定を取得します。
		/// </summary>
		public static SerializableProperty<string> UserStyleSheet { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming, Properties.Settings.Default.OverrideStyleSheet) { AutoSave = true };

		/// <summary>
		/// タスク バー インジケーターとして使用するプラグイン機能を識別する ID を取得します。
		/// </summary>
		public static SerializableProperty<string> TaskbarProgressSource { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming, "") { AutoSave = true };

		/// <summary>
		/// 出撃中にタスク バー インジケーターとして使用するプラグイン機能を識別する ID を取得します。
		/// </summary>
		public static SerializableProperty<string> TaskbarProgressSourceWhenSortie { get; }
			= new SerializableProperty<string>(GetKey(), Providers.Roaming, "") { AutoSave = true };

		/// <summary>
		/// 次回起動時にブラウザー キャッシュを削除するかどうかを示す設定値を取得します。
		/// </summary>
		public static SerializableProperty<bool> ClearCacheOnNextStartup { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Local) { AutoSave = true, };

		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(GeneralSettings) + "." + propertyName;
		}
	}
}
