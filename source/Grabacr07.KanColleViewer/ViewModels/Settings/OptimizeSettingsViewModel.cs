using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;
using Grabacr07.KanColleViewer.Models.Settings;
using Grabacr07.KanColleViewer.Properties;
using Livet;
using Livet.Messaging.IO;
using MetroTrilithon.Mvvm;
using Grabacr07.KanColleWrapper;

namespace Grabacr07.KanColleViewer.ViewModels.Settings
{
	public class OptimizeSettingsViewModel : ViewModel
	{
		public OptimizeSettingsViewModel()
		{
		}

		public void RequestGC()
		{
			GCWorker.GCRequest();
		}
	}
}
