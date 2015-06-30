using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class NotifierViewModel : PluginViewModel
	{
		private readonly INotifier notifier;
		
		public NotifierViewModel(Plugin plugin, IEnumerable<INotifier> notifiers = null)
			: base(plugin)
		{
			this.notifier = new AggregateNotifier(notifiers ?? plugin.OfType<INotifier>());
		}

		public void Test()
		{
			this.notifier.Show("テスト", "これはテスト通知です。", Application.Current.MainWindowViewModel.Activate, ex => this.ErrorMessage = ex.Message);
		}
	}
}
