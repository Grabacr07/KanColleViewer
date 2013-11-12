using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grabacr07.KanColleWrapper;
using Livet;
using Livet.EventListeners;

namespace Grabacr07.KanColleViewer.ViewModels
{
	public class MainWindowViewModel : WindowViewModel
	{
		#region Navigator 変更通知プロパティ

		private NavigatorViewModel _Navigator;

		public NavigatorViewModel Navigator
		{
			get { return this._Navigator; }
			private set
			{
				if (this._Navigator != value)
				{
					this._Navigator = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region NotificationMessage 変更通知プロパティ

		private string _NotificationMessage;

		public string NotificationMessage
		{
			get { return this._NotificationMessage; }
			set
			{
				if (this._NotificationMessage != value)
				{
					this._NotificationMessage = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		#region Content 変更通知プロパティ

		private ViewModel _Content;

		public ViewModel Content
		{
			get { return this._Content; }
			set
			{
				if (this._Content != value)
				{
					this._Content = value;
					this.RaisePropertyChanged();
				}
			}
		}

		#endregion

		public MainWindowViewModel()
		{
			this.Title = "提督業も忙しい！";
			this.Navigator = new NavigatorViewModel();

			this.Content = new NotStartedViewModel();
			//this.Content = new KanColleMonitorViewModel();
			this.CompositeDisposable.Add(new PropertyChangedEventListener(KanColleClient.Current)
			{
				{ "IsStarted", (sender, args) => this.Content = KanColleClient.Current.IsStarted ? new KanColleMonitorViewModel() : new NotStartedViewModel() as ViewModel },
			});
		}
	}
}
