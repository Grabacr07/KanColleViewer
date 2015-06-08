using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class PluginViewModel : ViewModel
	{
		protected IPlugin Plugin { get; private set; }

		private IPluginMetadata Metadata { get; set; }


		public string Title
		{
			get { return this.Metadata.Title; }
		}

		public string Description
		{
			get { return this.Metadata.Description; }
		}

		public string Author
		{
			get { return this.Metadata.Author; }
		}

		public string Version
		{
			get { return this.Metadata.Version; }
		}

		public object Settings
		{
			get { return this.Plugin.GetSettingsView(); }
		}


		public PluginViewModel(Lazy<IPlugin, IPluginMetadata> plugin)
		{
			this.Plugin = plugin.Value;
			this.Metadata = plugin.Metadata;
		}

		public PluginViewModel(IPlugin plugin, IPluginMetadata metadata)
		{
			this.Plugin = plugin;
			this.Metadata = metadata;
		}

		public void OpenSettings()
		{

		}
	}
}
