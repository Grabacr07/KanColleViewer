using Grabacr07.KanColleViewer.Composition;
using System;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class PluginViewModel : PluginViewModelBase<IPlugin>
	{
		public PluginViewModel(Lazy<IPlugin, IPluginMetadata> plugin) : base(plugin) { }
	}
}
