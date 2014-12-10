using Grabacr07.KanColleViewer.Composition;
using System;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class ToolViewModel : PluginViewModelBase<IToolPlugin>
	{
		public ToolViewModel(Lazy<IToolPlugin, IPluginMetadata> plugin) : base(plugin) { }

		public string ToolName
		{
			get { return this.Plugin.ToolName; }
		}

		public object View
		{
			get { return this.Plugin.GetToolView(); }
		}

		public override string ToString()
		{
			return this.Title;
		}
	}
}
