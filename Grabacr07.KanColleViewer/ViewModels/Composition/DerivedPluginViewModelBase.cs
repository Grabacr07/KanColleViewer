using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public abstract class DerivedPluginViewModelBase<TPlugin> : PluginViewModel where TPlugin : IPlugin
	{
		protected new TPlugin Plugin { get; private set; }

		protected DerivedPluginViewModelBase(Lazy<TPlugin, IPluginMetadata> plugin)
			: base(plugin.Value, plugin.Metadata)
		{
			this.Plugin = plugin.Value;
		}
	}
}
