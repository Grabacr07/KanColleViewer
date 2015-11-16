using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Plugins.Models;
using Grabacr07.KanColleViewer.Plugins.ViewModels;
using Grabacr07.KanColleViewer.Plugins.Views;
using MetroTrilithon.Serialization;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.Plugins
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ISettings))]
	[ExportMetadata("Guid", "E1F5EF36-18F5-42D5-896F-9476CD2F3128")]
	[ExportMetadata("Title", "AudibleNotifications")]
	[ExportMetadata("Description", "Audible notifications for KanColleViewer!; does not provide visual notifications.")]
	[ExportMetadata("Version", "1.1")]
	[ExportMetadata("Author", "@Xiatian")]

	public class AudibleNotifications : IPlugin, ISettings
	{
		private AudibleNotificationsSettingsViewModel viewModel;

		public Size SettingsSize => new Size(400, 575);

		public static List<string> Types { get; } = new List<string>();

		public static string LocationDefault => "Default";

		public static AudibleNotifierSettings Settings { get; private set; }

		public static bool IsWindows8OrGreater { get; set; }

		public void Initialize()
		{
			var version = Environment.OSVersion.Version;

			IsWindows8OrGreater = (version.Major == 6 && version.Minor >= 2) || version.Major > 6;

			Types.Add(LocationDefault);
			Types.AddRange(typeof(Notification.Types).GetProperties().Select(property => property.Name).ToList());

			Settings = new AudibleNotifierSettings(Types);

			this.viewModel = new AudibleNotificationsSettingsViewModel();
		}

		object ISettings.View => new AudibleNotificationsSettings { DataContext = this.viewModel, };

	}
}
