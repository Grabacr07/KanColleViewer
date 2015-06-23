using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public abstract class PluginViewModel<TContract> : PluginViewModel where TContract : class
	{
		protected TContract Function { get; private set; }

		protected PluginViewModel(Plugin plugin, TContract function = null)
			: base(plugin)
		{
			this.Function = function ?? plugin.OfType<TContract>().First();
		}
	}
}
