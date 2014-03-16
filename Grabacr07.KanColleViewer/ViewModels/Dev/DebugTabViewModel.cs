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
<<<<<<< HEAD
			get { return Properties.Resources.Debug; }
			protected set { throw new NotImplementedException(); }
=======
			this.Name = "Debug";
>>>>>>> parent of 6913fb4... 言語設定対応
		}


		public void Notify()
		{
			WindowsNotification.Notifier.Show(Properties.Resources.Debug_NotificationMessage_Title, Properties.Resources.Debug_NotificationMessage, () => App.ViewModelRoot.Activate());
		}

	}
}
