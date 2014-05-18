using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Contents
{
	public class ToolsViewModel : TabItemViewModel
	{
		public override string Name
		{
			get { return "ツール"; }
			protected set { throw new NotImplementedException(); }
		}

		#region Counters 変更通知プロパティ

		private ObservableCollection<CounterBase> _Counters;

		public ObservableCollection<CounterBase> Counters
		{
			get { return this._Counters; }
			set
			{
				if (this._Counters != value)
				{
					this._Counters = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion


		public ToolsViewModel()
		{
			this.Counters = new ObservableCollection<CounterBase>
			{
				new SupplyCounter(KanColleClient.Current.Proxy),
				new ItemDestroyCounter(KanColleClient.Current.Proxy),
				new MissionCounter(KanColleClient.Current.Proxy),
			};
		}
	}
}
