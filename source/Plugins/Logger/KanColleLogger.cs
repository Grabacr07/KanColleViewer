using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;
using Grabacr07.KanColleWrapper;

namespace Logger
{
	[Export(typeof(IPlugin))]
	[Export(typeof(ITool))]
	[ExportMetadata("Guid", "B33A80D8-F529-430C-AB56-1B6C486AA4D9")]
	[ExportMetadata("Title", "KanColleLogger")]
	[ExportMetadata("Description", "File logging back-end")]
	[ExportMetadata("Version", "1.0")]
	[ExportMetadata("Author", "@Xiatian")]
	public class KanColleCounter : IPlugin, ITool
	{
		private LoggerViewModel viewmodel;

		string ITool.Name => "Logger";

		object ITool.View => new LoggerView { DataContext = this.viewmodel, };

		public void Initialize()
		{
			this.viewmodel = new LoggerViewModel
			{
				Loggers = new ObservableCollection<LoggerBase>
					{
						new ItemLog(KanColleClient.Current.Proxy),
						new ConstructionLog(KanColleClient.Current.Proxy),
						new BattleLog(KanColleClient.Current.Proxy),
						new MaterialsLog(KanColleClient.Current.Proxy),
					}
			};
		}
	}
}
