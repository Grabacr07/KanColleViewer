using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetroTrilithon.Mvvm;
using Livet.Messaging.Windows;
using Grabacr07.KanColleWrapper;
using Grabacr07.KanColleViewer.ViewModels.Contents.Fleets;
using Grabacr07.KanColleViewer.ViewModels.Catalogs;

namespace Grabacr07.KanColleViewer.ViewModels.Catalogs
{
	public class PresetFleetDeleteWindowViewModel : WindowViewModel
	{
		private PresetFleetWindowViewModel _ViewModel;
		private PresetFleetData _TargetFleet;

		public PresetFleetDeleteWindowViewModel(PresetFleetWindowViewModel viewmodel, PresetFleetData targetfleet)
		{
			this._ViewModel = viewmodel;
			this._TargetFleet = targetfleet;
		}
		
		public void DeleteFleet(string delete)
		{
			if(bool.Parse(delete))
				_ViewModel.DeleteFleet(_TargetFleet);

			Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
		}
	}
}
