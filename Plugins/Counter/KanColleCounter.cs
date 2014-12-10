using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace Counter
{
	[Export(typeof(IToolPlugin))]
	[ExportMetadata("Title", "KanColleCounter")]
	[ExportMetadata("Description", "シンプルな回数カウント機能を提供します。")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Grabacr07")]
	public class KanColleCounter : IToolPlugin
	{
		private readonly CounterViewModel viewmodel = new CounterViewModel
		{
			Counters = new ObservableCollection<CounterBase>
			{
				new SupplyCounter(KanColleClient.Current.Proxy),
				new ItemDestroyCounter(KanColleClient.Current.Proxy),
				new MissionCounter(KanColleClient.Current.Proxy),
			}
		};

		public string ToolName
		{
			get { return "Counter"; }
		}

		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
			return new CounterView { DataContext = this.viewmodel, };
		}
	}
}
