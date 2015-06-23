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
		protected Plugin Plugin { get; private set; }

		public string Title
		{
			get { return this.Plugin.Metadata.Title; }
		}

		public string Description
		{
			get { return this.Plugin.Metadata.Description; }
		}

		public string Author
		{
			get { return this.Plugin.Metadata.Author; }
		}

		public string Version
		{
			get { return this.Plugin.Metadata.Version; }
		}

		public object Settings
		{
			get { return this.Plugin.OfType<ISettings>().FirstOrDefault(); }
		}


		public PluginViewModel(Plugin plugin)
		{
			this.Plugin = plugin;
		}

		public void OpenSettings()
		{

		}
	}
}
