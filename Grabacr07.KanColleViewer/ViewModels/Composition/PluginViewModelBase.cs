using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public abstract class PluginViewModelBase<TPlugin> : ViewModel where TPlugin : IPlugin
	{
		protected TPlugin Plugin { get; private set; }

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


		protected PluginViewModelBase(Lazy<TPlugin, IPluginMetadata> plugin)
		{
			this.Plugin = plugin.Value;
			this.Metadata = plugin.Metadata;
		}
	}
}
