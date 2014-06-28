using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;

namespace Counter
{
	[Export(typeof(IToolPlugin))]
	public class KanColleCounter : IToolPlugin
	{
		public string ToolName
		{
			get { return "Counters"; }
		}

		public object GetSettingsView()
		{
			return null;
		}

		public object GetToolView()
		{
			var vm = new CounterViewModel
			{
				Counters = new ObservableCollection<CounterBase>
				{
					new SupplyCounter(KanColleClient.Current.Proxy),
					new ItemDestroyCounter(KanColleClient.Current.Proxy),
					new MissionCounter(KanColleClient.Current.Proxy),
				}
			};

			return new CounterView { DataContext = vm };
		}
	}
}
