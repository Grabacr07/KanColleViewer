using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleViewer.Composition;

namespace Grabacr07.KanColleViewer.ViewModels.Composition
{
	public class NotifierViewModel : PluginViewModelBase<INotifier>
	{
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

		public NotifierViewModel(Lazy<INotifier, IPluginMetadata> plugin) : base(plugin)
		{
			this.ErrorMessage = null;
		}

		public void Test()
		{
			this.Plugin.Show(NotifyType.Other, "テスト", "これはテスト通知です。", App.ViewModelRoot.Activate, ex => this.ErrorMessage = ex.Message);
		}
	}
}
