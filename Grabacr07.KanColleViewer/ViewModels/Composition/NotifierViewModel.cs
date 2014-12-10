using Grabacr07.KanColleViewer.Composition;
using System;

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
			this.Plugin.Show(NotifyType.Other, "테스트", "테스트 알림입니다.", App.ViewModelRoot.Activate, ex => this.ErrorMessage = ex.Message);
		}
	}
}
