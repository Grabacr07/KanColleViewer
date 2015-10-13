using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public class TranslatorSettings : SettingsHost
	{
		/// <summary>
		/// 번역을 활성화합니다
		/// </summary>
		public static SerializableProperty<bool> EnableTranslations { get; }
			= new SerializableProperty<bool>(GetKey(), Providers.Local, true);

		/// <summary>
		/// 번역이 되지 않은 텍스트가 있는 경우 해당 데이터를 xml에 추가 입력합니다
		/// </summary>
		public static SerializableProperty<bool> EnableAddUntranslated { get; }
			= new SerializableProperty<bool>(GetKey(),Providers.Local, true);


		private static string GetKey([CallerMemberName] string propertyName = "")
		{
			return nameof(TranslatorSettings) + "." + propertyName;
		}
	}
}
