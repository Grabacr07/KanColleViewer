using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Livet;

namespace Counter
{
	public class CounterViewModel : ViewModel
	{
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

		public CounterViewModel()
		{
		}
	}
}
