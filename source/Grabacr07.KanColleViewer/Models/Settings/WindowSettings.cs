using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public class WindowSettings : SettingsHost
	{
		/// <summary>
		/// ウィンドウを常に最前面に表示するかどうかを示す設定値を取得します。
		/// </summary>
		public SerializableProperty<bool> TopMost => this.Cache(key => new SerializableProperty<bool>(key, Providers.Local, false));


		#region infrastructures

		protected override string CategoryName { get; }

		protected WindowSettings() : this(null) { }

		public WindowSettings(string key)
		{
			this.CategoryName = key ?? this.GetType().Name;
		}

		#endregion
	}
}
