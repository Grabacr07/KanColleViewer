using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class PluginViewModel : PluginViewModelBase<IPlugin>
	{
		public PluginViewModel(Lazy<IPlugin, IPluginMetadata> plugin) : base(plugin) { }
	}
}
