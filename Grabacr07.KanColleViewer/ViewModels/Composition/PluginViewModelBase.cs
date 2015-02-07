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

		private IPluginMetadata Metadata { get; }


		public string Title => this.Metadata.Title;

	    public string Description => this.Metadata.Description;

	    public string Author => this.Metadata.Author;

	    public string Version => this.Metadata.Version;


	    protected PluginViewModelBase(Lazy<TPlugin, IPluginMetadata> plugin)
		{
			this.Plugin = plugin.Value;
			this.Metadata = plugin.Metadata;
		}
	}
}
