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
		public static string RoamingFilePath { get; } = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"grabacr.net", "KanColleViewer", "Settings.xaml");

		public static string LocalFilePath { get; } = Path.Combine(
			Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
			"grabacr.net", "KanColleViewer", "Settings.xaml");

		public static ISerializationProvider Roaming { get; } = new FileSettingsProvider(RoamingFilePath);

		public static ISerializationProvider Local { get; } = new FileSettingsProvider(LocalFilePath);
	}
}
