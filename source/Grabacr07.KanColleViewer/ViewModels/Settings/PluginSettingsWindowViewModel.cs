using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using MetroTrilithon.Mvvm;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class PluginSettingsWindowViewModel : WindowViewModel
	{
		private readonly ISettings settings;

		public object Content => this.settings.View;

		public PluginSettingsWindowViewModel(ISettings settings, string title)
		{
			this.Title = $"{title} プラグイン設定";
			this.settings = settings;
		}
	}
}
