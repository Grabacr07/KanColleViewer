using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Models;

namespace Grabacr07.KanColleViewer.ViewModels.Dev
{
	public class DebugTabViewModel : TabItemViewModel
	{
		public DebugTabViewModel()
		{
			this.Name = "Debug";
		}


		public void Notify()
		{
			WindowsNotification.Notifier.Show("テスト", "これはテスト通知です。", () => App.ViewModelRoot.Activate());
		}

	}
}
