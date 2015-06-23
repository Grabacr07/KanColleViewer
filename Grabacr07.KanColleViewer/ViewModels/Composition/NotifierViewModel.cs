using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class NotifierViewModel : PluginViewModel
	{
		private readonly AggregateNotifier notifier;

		#region ErrorMessage 変更通知プロパティ

		private string _ErrorMessage;

		public string ErrorMessage
		{
			get { return this._ErrorMessage; }
			set
			{
				if (this._ErrorMessage != value)
				{
					this._ErrorMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public NotifierViewModel(Plugin plugin, IEnumerable<INotifier> notifiers = null)
			: base(plugin)
		{
			this.notifier = new AggregateNotifier(notifiers ?? plugin.OfType<INotifier>());
		}

		public void Test()
		{
			this.notifier.Show(NotifyType.Other, "テスト", "これはテスト通知です。", App.ViewModelRoot.Activate, ex => this.ErrorMessage = ex.Message);
		}
	}
}
