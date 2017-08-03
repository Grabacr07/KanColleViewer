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
		/// 윈도우를 항상 위에 표시할지 여부를 설정합니다.
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
