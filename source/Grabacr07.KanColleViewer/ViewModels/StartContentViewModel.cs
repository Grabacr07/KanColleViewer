using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models.Settings;
using Livet;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class StartContentViewModel : ViewModel
	{
		public NavigatorViewModel Navigator { get; }


		public bool ClearCacheOnNextStartup
		{
			get => GeneralSettings.ClearCacheOnNextStartup.Value;
			set => GeneralSettings.ClearCacheOnNextStartup.Value = value;
		}


		public StartContentViewModel(NavigatorViewModel navigator)
		{
			this.Navigator = navigator;
		}
	}
}
