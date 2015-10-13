using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public class UpdaterSettings : SettingsHost
	{
		/// <summary>
		/// 업데이트 알림을 활성화합니다
		/// </summary>
		public static SerializableProperty<bool> EnableUpdateNotification { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Local, true);

		/// <summary>
		/// 
		/// </summary>
		public static SerializableProperty<bool> EnableUpdateTransOnStart { get; }
			= new SerializableProperty<bool>(GetKey(), true);


		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(UpdaterSettings) + "." + propertyName;
		}
	}
}
