using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public abstract class SettingsHost
	{
		private static readonly Dictionary<Type, SettingsHost> instances = new Dictionary<Type, SettingsHost>();
		private readonly Dictionary<string, object> cachedProperties = new Dictionary<string, object>();

		protected virtual string CategoryName => this.GetType().Name;

		protected SettingsHost()
		{
			instances[this.GetType()] = this;
		}

		/// <summary>
		/// 現在のインスタンスにキャッシュされている <see cref="SerializableProperty{T}"/>
		/// を取得します。 キャッシュがない場合は <see cref="create"/> に従って生成します。
		/// </summary>
		/// <returns></returns>
		protected SerializableProperty<T> Cache<T>(Func<string, SerializableProperty<T>> create, [CallerMemberName] string propertyName = "")
		{
			var key = this.CategoryName + "." + propertyName;

			object obj;
			if (this.cachedProperties.TryGetValue(key, out obj) && obj is SerializableProperty<T>) return (SerializableProperty<T>)obj;

			var property = create(key);
			this.cachedProperties[key] = property;

			return property;
		}


		#region Load / Save

		public static void Load()
		{
#pragma warning disable 612
			// 古い設定が存在する可能性があるので、読んでおく
			// (ただし、1 度読んだら新しい方に移行するので保存はしない
			Migration._Settings.Load();
#pragma warning restore 612

			#region const message

			const string message = @"設定ファイル ({0}) の読み込みに失敗しました。このファイルを削除することで解決する可能性があります。

エラーの詳細: {1}";

			#endregion

			try
			{
				Providers.Local.Load();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(message, Providers.LocalFilePath, ex.Message), "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
				throw;
			}

			try
			{
				Providers.Roaming.Load();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(message, Providers.RoamingFilePath, ex.Message), "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
				throw;
			}
		}

		public static void Save()
		{
			#region const message

			const string message = @"設定ファイル ({0}) の保存に失敗しました。

エラーの詳細: {1}";

			#endregion

			try
			{
				Providers.Local.Save();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(message, Providers.LocalFilePath, ex.Message), "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
				throw;
			}

			try
			{
				Providers.Roaming.Save();
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(message, Providers.RoamingFilePath, ex.Message), "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
				throw;
			}
		}

		#endregion

		/// <summary>
		/// <typeparamref name="T"/> 型の設定オブジェクトの唯一のインスタンスを取得します。
		/// </summary>
		public static T Instance<T>() where T : SettingsHost, new()
		{
			SettingsHost host;
			return instances.TryGetValue(typeof(T), out host) ? (T)host : new T();
		}
	}
}
