using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Grabacr07.KanColleViewer.Models.Settings;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Plugins.Models
{
	public class AudibleNotifierSettings : SettingsHost
	{
		public SerializableProperty<string> Location
			=> this.Cache(key => new SerializableProperty<string>(key, Providers.Roaming, Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)) { AutoSave = true });

		public SerializableProperty<int> Volume
			=> this.Cache(key => new SerializableProperty<int>(key, Providers.Roaming, 100) { AutoSave = true });

		public Dictionary<string, SerializableProperty<bool>> TypeSettings = new Dictionary<string, SerializableProperty<bool>>();

		public AudibleNotifierSettings(IEnumerable<string> types)
		{
			foreach (var type in types)
			{
				TypeSettings.Add(type, this.Cache(key => new SerializableProperty<bool>(TypeToPropertyName(type), Providers.Roaming, true) { AutoSave = true }, TypeToPropertyName(type)));
			}
		}
		public static string TypeToPropertyName(string type)
			=> nameof(AudibleNotifierSettings) + ".Types." + type;
	}
}