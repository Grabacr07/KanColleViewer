using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MetroTrilithon.Serialization;

namespace Grabacr07.KanColleViewer.Models.Settings
{
	public static class Providers
	{

		private static readonly string roamingFilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"grabacr.net", "KanColleViewer", "Settings.xaml");

		private static readonly string localFilePath = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"grabacr.net", "KanColleViewer", "Settings.xaml");

		public static ISerializationProvider Roaming { get; } = new FileSettingsProvider(roamingFilePath);
		public static ISerializationProvider Local { get; } = new FileSettingsProvider(localFilePath);
	}
}
