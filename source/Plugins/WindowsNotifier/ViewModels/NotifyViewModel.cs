using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleViewer.Plugins.Properties;
using Livet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Plugins.ViewModels
{
	public class NotifyViewModel : ViewModel
	{
		private static readonly Settings settings = Settings.Default;
		public float CustomVolume
		{
			get { return settings.CustomVolume; }
			set
			{
				if (settings.CustomVolume == value) return;
				settings.CustomVolume = value;
				settings.Save();
				this.RaisePropertyChanged();
			}
		}
	}
}
