using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Controls.Globalization;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class PluginSettingsWindowViewModel : WindowViewModel
	{
		private readonly ISettings settings;

		private readonly Size defaultSize = new Size(400, 350);

		public object Content => this.settings.View;

		public Size Size
		{
			get { return GetSize(); }
			set { }
		}

		public PluginSettingsWindowViewModel(ISettings settings, string title)
		{
			this.Title = string.Format(KanColleViewer.Properties.Resources.Settings_Plugins_PluginSettingsTitle, title);
			this.settings = settings;
		}

		private Size GetSize()
		{
			PropertyInfo pInfo = settings.GetType().GetProperty("SettingsSize");

			if ((pInfo == null) || (pInfo.PropertyType != typeof(Size))) return defaultSize;

			return (Size)pInfo.GetValue(settings);
		}
	}
}
